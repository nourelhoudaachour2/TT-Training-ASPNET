using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models;
using Training_tunisie_telecome.Models.ViewModels;

namespace Training_tunisie_telecome.Controllers
{
    public class EmployeePortalController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeePortalController(AppDbContext context)
        {
            _context = context;
        }

        private int? GetEmployeeId()
        {
            if (HttpContext.Session.GetString("role") != "Employee")
                return null;

            return HttpContext.Session.GetInt32("employeeId");
        }

        private IActionResult RedirectToLogin()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        private async Task UpdateNotificationCounter(int employeeId)
        {
            var unread = await _context.Notifications
                .AsNoTracking()
                .CountAsync(n => n.EmployeeId == employeeId && n.Status == "Non lu");

            HttpContext.Session.SetInt32("employeeUnreadNotifications", unread);
        }

        private static string GetPresenceStatus(SessionParticipant participant)
        {
            var lastAttendance = participant.Attendances
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.CreatedAt)
                .FirstOrDefault();

            if (lastAttendance != null)
                return lastAttendance.Status;

            if (participant.Present == true)
                return "Présent";

            if (participant.Present == false)
                return "Absent";

            return "Non renseigné";
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return RedirectToLogin();

            var employee = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Domain)
                .FirstOrDefaultAsync(e => e.Id == employeeId.Value);

            if (employee == null)
                return RedirectToLogin();

            var participants = await _context.SessionParticipants
                .AsNoTracking()
                .Include(p => p.Attendances)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Formation)
                .Where(p => p.EmployeeId == employee.Id)
                .ToListAsync();

            var participantIds = participants.Select(p => p.Id).ToList();

            var completedEvaluations = await _context.Evaluations
                .AsNoTracking()
                .CountAsync(e => participantIds.Contains(e.SessionParticipantId));

            var attendanceRows = participants
                .SelectMany(p => p.Attendances)
                .Where(a => a.Status == "Présent" || a.Status == "Absent" || a.Status == "Excusé")
                .ToList();

            var presentCount = attendanceRows.Count(a => a.Status == "Présent");
            var attendanceRate = attendanceRows.Count == 0
                ? 0
                : Math.Round((double)presentCount * 100 / attendanceRows.Count, 1);

            var today = DateTime.Today;
            var completed = participants
                .Where(p => p.SessionFormation != null && p.SessionFormation.DateEnd < today)
                .ToList();

            var model = new EmployeeProfileViewModel
            {
                EmployeeId = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Matricule = employee.Matricule,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Grade = employee.Grade,
                Residence = employee.Residence,
                Domain = employee.Domain?.Name ?? "Non renseigné",
                ImagePath = employee.ImagePath,
                CompletedFormations = completed.Count,
                TotalTrainingHours = completed.Sum(p => p.SessionFormation?.Formation?.Duration ?? 0),
                AttendanceRate = attendanceRate,
                CompletedEvaluations = completedEvaluations
            };

            await UpdateNotificationCounter(employee.Id);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Formations()
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return RedirectToLogin();

            var today = DateTime.Today;

            var participants = await _context.SessionParticipants
                .AsNoTracking()
                .Include(p => p.ReplacementEmployee)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Formation)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Trainer)
                .Where(p => p.EmployeeId == employeeId.Value &&
                            p.SessionFormation != null &&
                            p.SessionFormation.DateEnd >= today)
                .OrderBy(p => p.SessionFormation!.DateStart)
                .ToListAsync();

            var colleagues = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Domain)
                .Where(e => e.Id != employeeId.Value)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            var model = new EmployeeFormationsPageViewModel
            {
                Upcoming = participants.Select(p => new EmployeeFormationItemViewModel
                {
                    ParticipantId = p.Id,
                    FormationId = p.SessionFormation!.FormationId,
                    SessionId = p.SessionFormation.Id,
                    Title = p.SessionFormation.Formation?.Title ?? "Formation",
                    Module = p.SessionFormation.Formation?.Module ?? string.Empty,
                    DateStart = p.SessionFormation.DateStart,
                    DateEnd = p.SessionFormation.DateEnd,
                    Location = p.SessionFormation.Location,
                    Trainer = p.SessionFormation.Trainer == null
                        ? "Non renseigné"
                        : $"{p.SessionFormation.Trainer.FirstName} {p.SessionFormation.Trainer.LastName}",
                    Confirmed = p.Confirmed,
                    ReplacementEmployeeId = p.ReplacementEmployeeId,
                    ReplacementEmployeeName = p.ReplacementEmployee == null
                        ? null
                        : $"{p.ReplacementEmployee.FirstName} {p.ReplacementEmployee.LastName}"
                }).ToList(),

                AvailableReplacements = colleagues
                    .Select(e => new EmployeeReplacementOptionViewModel
                    {
                        EmployeeId = e.Id,
                        FullName = $"{e.FirstName} {e.LastName}",
                        Matricule = e.Matricule,
                        Domain = e.Domain?.Name ?? "Non renseigné"
                    })
                    .ToList()
            };

            await UpdateNotificationCounter(employeeId.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestReplacement(
            int participantId,
            int replacementEmployeeId)
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return RedirectToLogin();

            var participant = await _context.SessionParticipants
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Formation)
                .FirstOrDefaultAsync(p =>
                    p.Id == participantId &&
                    p.EmployeeId == employeeId.Value);

            if (participant == null ||
                participant.SessionFormation == null)
            {
                TempData["ReplacementError"] =
                    "La formation sélectionnée est introuvable.";

                return RedirectToAction(nameof(Formations));
            }

            if (participant.SessionFormation.DateStart.Date < DateTime.Today)
            {
                TempData["ReplacementError"] =
                    "La demande de remplacement n'est plus disponible pour cette formation.";

                return RedirectToAction(nameof(Formations));
            }

            if (replacementEmployeeId == employeeId.Value)
            {
                TempData["ReplacementError"] =
                    "Vous devez sélectionner un autre collègue.";

                return RedirectToAction(nameof(Formations));
            }

            var replacement = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == replacementEmployeeId);

            if (replacement == null)
            {
                TempData["ReplacementError"] =
                    "Le collègue sélectionné est introuvable.";

                return RedirectToAction(nameof(Formations));
            }

            var alreadyParticipant = await _context.SessionParticipants
                .AsNoTracking()
                .AnyAsync(p =>
                    p.SessionFormationId == participant.SessionFormationId &&
                    p.EmployeeId == replacementEmployeeId);

            if (alreadyParticipant)
            {
                TempData["ReplacementError"] =
                    "Ce collègue participe déjà à cette formation.";

                return RedirectToAction(nameof(Formations));
            }

            participant.ReplacementEmployeeId = replacementEmployeeId;

            _context.Notifications.Add(new Notification
            {
                EmployeeId = employeeId.Value,
                Type = "Remplacement",
                Message =
                    $"Votre demande de remplacement pour la formation " +
                    $"« {participant.SessionFormation.Formation?.Title ?? "Formation"} » " +
                    $"a été enregistrée. Collègue proposé : " +
                    $"{replacement.FirstName} {replacement.LastName}.",
                SentDate = DateTime.Now,
                Status = "Non lu"
            });

            await _context.SaveChangesAsync();

            TempData["ReplacementSuccess"] =
                $"Demande enregistrée. {replacement.FirstName} {replacement.LastName} " +
                $"apparaîtra comme remplaçant proposé chez le Responsable RH.";

            await UpdateNotificationCounter(employeeId.Value);

            return RedirectToAction(nameof(Formations));
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return RedirectToLogin();

            var today = DateTime.Today;

            var participants = await _context.SessionParticipants
                .AsNoTracking()
                .Include(p => p.Attendances)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Formation)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Trainer)
                .Where(p => p.EmployeeId == employeeId.Value &&
                            p.SessionFormation != null &&
                            p.SessionFormation.DateEnd < today)
                .OrderByDescending(p => p.SessionFormation!.DateEnd)
                .ToListAsync();

            var participantIds = participants.Select(p => p.Id).ToList();
            var evaluations = await _context.Evaluations
                .AsNoTracking()
                .Where(e => participantIds.Contains(e.SessionParticipantId))
                .ToListAsync();

            var model = new EmployeeHistoryPageViewModel
            {
                Completed = participants.Select(p => new EmployeeFormationItemViewModel
                {
                    ParticipantId = p.Id,
                    FormationId = p.SessionFormation!.FormationId,
                    SessionId = p.SessionFormation.Id,
                    Title = p.SessionFormation.Formation?.Title ?? "Formation",
                    Module = p.SessionFormation.Formation?.Module ?? string.Empty,
                    DateStart = p.SessionFormation.DateStart,
                    DateEnd = p.SessionFormation.DateEnd,
                    Location = p.SessionFormation.Location,
                    Trainer = p.SessionFormation.Trainer == null
                        ? "Non renseigné"
                        : $"{p.SessionFormation.Trainer.FirstName} {p.SessionFormation.Trainer.LastName}",
                    PresenceStatus = GetPresenceStatus(p),
                    EvaluationStatus = evaluations.Any(e => e.SessionParticipantId == p.Id)
                        ? "Complétée"
                        : "À compléter"
                }).ToList()
            };

            await UpdateNotificationCounter(employeeId.Value);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Evaluations()
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return RedirectToLogin();

            var participants = await _context.SessionParticipants
                .AsNoTracking()
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Formation)
                .Where(p => p.EmployeeId == employeeId.Value && p.SessionFormation != null)
                .ToListAsync();

            var participantIds = participants.Select(p => p.Id).ToList();
            var evaluations = await _context.Evaluations
                .AsNoTracking()
                .Where(e => participantIds.Contains(e.SessionParticipantId))
                .ToListAsync();

            var today = DateTime.Today;
            var pending = new List<EmployeeEvaluationItemViewModel>();
            var completed = new List<EmployeeEvaluationItemViewModel>();

            foreach (var participant in participants)
            {
                var session = participant.SessionFormation!;
                var title = session.Formation?.Title ?? "Formation";

                if (session.DateStart.Date >= today)
                {
                    var exists = evaluations.Any(e =>
                        e.SessionParticipantId == participant.Id && e.Type == "Préalable");

                    var item = new EmployeeEvaluationItemViewModel
                    {
                        ParticipantId = participant.Id,
                        FormationTitle = title,
                        DateStart = session.DateStart,
                        DateEnd = session.DateEnd,
                        Type = "Évaluation préalable",
                        Status = exists ? "Complétée" : "À remplir"
                    };

                    if (exists) completed.Add(item); else pending.Add(item);
                }

                if (session.DateEnd.Date < today)
                {
                    var exists = evaluations.Any(e =>
                        e.SessionParticipantId == participant.Id && e.Type == "À chaud");

                    var item = new EmployeeEvaluationItemViewModel
                    {
                        ParticipantId = participant.Id,
                        FormationTitle = title,
                        DateStart = session.DateStart,
                        DateEnd = session.DateEnd,
                        Type = "Évaluation à chaud",
                        Status = exists ? "Complétée" : "À remplir"
                    };

                    if (exists) completed.Add(item); else pending.Add(item);
                }
            }

            var model = new EmployeeEvaluationsPageViewModel
            {
                Pending = pending.OrderBy(e => e.DateStart).ToList(),
                Completed = completed.OrderByDescending(e => e.DateStart).ToList()
            };

            await UpdateNotificationCounter(employeeId.Value);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Absences()
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return RedirectToLogin();

            var participants = await _context.SessionParticipants
                .AsNoTracking()
                .Include(p => p.Attendances)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Formation)
                .Where(p => p.EmployeeId == employeeId.Value)
                .ToListAsync();

            var absences = participants
                .SelectMany(p => p.Attendances
                    .Where(a => a.Status == "Absent")
                    .Select(a => new EmployeeAbsenceItemViewModel
                    {
                        ParticipantId = p.Id,
                        FormationTitle = p.SessionFormation?.Formation?.Title ?? "Formation",
                        Date = a.Date,
                        Status = a.Status,
                        AbsenceStatus = p.AbsenceStatus,
                        JustificationFile = p.JustificationFile
                    }))
                .OrderByDescending(a => a.Date)
                .ToList();

            var model = new EmployeeAbsencesPageViewModel
            {
                Absences = absences
            };

            await UpdateNotificationCounter(employeeId.Value);
            return View(model);
        }
        // ================= UPLOAD ABSENCE JUSTIFICATION =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadJustification(
            int participantId,
            IFormFile file)
        {
            var employeeId = GetEmployeeId();

            if (employeeId == null)
                return RedirectToLogin();


            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Veuillez sélectionner un fichier PDF.";
                return RedirectToAction(nameof(Absences));
            }


            if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
            {
                TempData["Error"] = "Le fichier doit être un PDF.";
                return RedirectToAction(nameof(Absences));
            }


            var participant = await _context.SessionParticipants
                .FirstOrDefaultAsync(p =>
                    p.Id == participantId &&
                    p.EmployeeId == employeeId.Value);


            if (participant == null)
                return NotFound();



            var uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "justifications"
            );


            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);



            var fileName =
                Guid.NewGuid().ToString()
                + Path.GetExtension(file.FileName);



            var filePath = Path.Combine(
                uploadFolder,
                fileName
            );


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }



            participant.JustificationFile =
                "/uploads/justifications/" + fileName;


            participant.JustificationUploadedAt =
                DateTime.Now;


            participant.AbsenceStatus =
                "En attente";



            await _context.SaveChangesAsync();


            TempData["Success"] =
                "Votre justificatif a été envoyé avec succès.";


            return RedirectToAction(nameof(Absences));
        }
        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return RedirectToLogin();

            var notifications = await _context.Notifications
                .AsNoTracking()
                .Where(n => n.EmployeeId == employeeId.Value)
                .OrderByDescending(n => n.SentDate)
                .ToListAsync();

            var absenceParticipants = await _context.SessionParticipants
                .AsNoTracking()
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s!.Formation)
                .Where(p => p.EmployeeId == employeeId.Value && p.AbsenceStatus != null)
                .ToListAsync();

            string? ResolveAbsenceStatus(string message)
            {
                var match = absenceParticipants
                    .Where(p => !string.IsNullOrEmpty(p.SessionFormation?.Formation?.Title) &&
                                message.Contains(p.SessionFormation!.Formation!.Title))
                    .OrderByDescending(p => p.JustificationUploadedAt)
                    .FirstOrDefault();

                return match?.AbsenceStatus;
            }

            var model = new EmployeeNotificationsPageViewModel
            {
                Notifications = notifications.Select(n => new EmployeeNotificationItemViewModel
                {
                    Id = n.Id,
                    Type = n.Type,
                    Message = n.Message,
                    SentDate = n.SentDate,
                    Status = n.Status,

                    ActionUrl = n.Type == "Absence"
                        ? "/EmployeePortal/Absences"
                        : null,

                    AbsenceStatus = n.Type == "Absence"
                        ? ResolveAbsenceStatus(n.Message)
                        : null

                }).ToList(),
                UnreadCount = notifications.Count(n => n.Status == "Non lu")
            };

            HttpContext.Session.SetInt32("employeeUnreadNotifications", model.UnreadCount);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllNotificationsRead()
        {
            var employeeId = GetEmployeeId();
            if (employeeId == null)
                return RedirectToLogin();

            var notifications = await _context.Notifications
                .Where(n => n.EmployeeId == employeeId.Value && n.Status == "Non lu")
                .ToListAsync();

            foreach (var notification in notifications)
                notification.Status = "Lu";

            await _context.SaveChangesAsync();
            HttpContext.Session.SetInt32("employeeUnreadNotifications", 0);

            return RedirectToAction(nameof(Notifications));
        }
    }
}