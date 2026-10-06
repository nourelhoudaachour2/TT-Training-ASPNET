using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models.ViewModels;

namespace Training_tunisie_telecome.Controllers
{
    public class EmployeeDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeDashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("role") != "Employee")
            {
                return RedirectToAction("Login", "Auth");
            }

            var employeeId = HttpContext.Session.GetInt32("employeeId");

            if (employeeId == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth");
            }

            var employee = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Domain)
                .FirstOrDefaultAsync(e => e.Id == employeeId.Value);

            if (employee == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth");
            }

            var participants = await _context.SessionParticipants
                .AsNoTracking()
                .Include(p => p.Attendances)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Formation)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Trainer)
                .Where(p => p.EmployeeId == employee.Id)
                .ToListAsync();

            var participantIds = participants
                .Select(p => p.Id)
                .ToList();

            var evaluations = await _context.Evaluations
                .AsNoTracking()
                .Where(e => participantIds.Contains(e.SessionParticipantId))
                .ToListAsync();

            var unreadNotifications = await _context.Notifications
                .AsNoTracking()
                .CountAsync(n =>
                    n.EmployeeId == employee.Id &&
                    n.Status == "Non lu");

            HttpContext.Session.SetInt32(
                "employeeUnreadNotifications",
                unreadNotifications);

            var today = DateTime.Today;

            var attendanceRows = participants
                .SelectMany(p => p.Attendances)
                .Where(a =>
                    a.Status == "Présent" ||
                    a.Status == "Absent" ||
                    a.Status == "Excusé")
                .ToList();

            var presentCount = attendanceRows
                .Count(a => a.Status == "Présent");

            var attendanceRate = attendanceRows.Count == 0
                ? 0
                : Math.Round(
                    (double)presentCount * 100 / attendanceRows.Count,
                    1);

            var upcoming = participants
                .Where(p =>
                    p.SessionFormation != null &&
                    p.SessionFormation.DateStart.Date >= today)
                .OrderBy(p => p.SessionFormation!.DateStart)
                .ToList();

            var completed = participants
                .Where(p =>
                    p.SessionFormation != null &&
                    p.SessionFormation.DateEnd.Date < today)
                .ToList();

            int pendingEvaluations = 0;

            foreach (var participant in upcoming)
            {
                if (!evaluations.Any(e =>
                    e.SessionParticipantId == participant.Id &&
                    e.Type == "Préalable"))
                {
                    pendingEvaluations++;
                }
            }

            foreach (var participant in completed)
            {
                if (!evaluations.Any(e =>
                    e.SessionParticipantId == participant.Id &&
                    e.Type == "À chaud"))
                {
                    pendingEvaluations++;
                }
            }

            var model = new EmployeeDashboardViewModel
            {
                EmployeeId = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Matricule = employee.Matricule,
                Domain = employee.Domain?.Name ?? "Non renseigné",
                ImagePath = employee.ImagePath,
                AttendanceRate = attendanceRate,
                PlannedFormations = upcoming.Count,
                CompletedFormations = completed.Count,
                PendingEvaluations = pendingEvaluations,
                UnreadNotifications = unreadNotifications,
                UpcomingFormations = upcoming
                    .Take(3)
                    .Select(p => new EmployeeUpcomingFormationViewModel
                    {
                        ParticipantId = p.Id,
                        FormationId = p.SessionFormation!.FormationId,
                        SessionId = p.SessionFormation.Id,
                        Title = p.SessionFormation.Formation?.Title ?? "Formation",
                        DateStart = p.SessionFormation.DateStart,
                        DateEnd = p.SessionFormation.DateEnd,
                        Location = p.SessionFormation.Location,
                        Trainer = p.SessionFormation.Trainer == null
                            ? "Non renseigné"
                            : $"{p.SessionFormation.Trainer.FirstName} {p.SessionFormation.Trainer.LastName}",
                        Confirmed = p.Confirmed
                    })
                    .ToList()
            };

            return View(model);
        }
    }
}