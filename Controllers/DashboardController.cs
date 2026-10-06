using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models.ViewModels;
using Training_tunisie_telecome.Models;

namespace Training_tunisie_telecome.Controllers
{
    public class DashboardController : Controller
    {

        private readonly AppDbContext _context;


        public DashboardController(AppDbContext context)
        {
            _context = context;
        }





        public async Task<IActionResult> Index()
        {

            var model = new DashboardViewModel();



            // ================= KPI =================


            model.TotalFormations =
                await _context.Formations.CountAsync();



            model.TotalSessions =
                await _context.SessionFormations.CountAsync();



            model.TotalEmployees =
                await _context.Employees.CountAsync();



            model.TotalParticipants =
                await _context.SessionParticipants.CountAsync();



            model.TotalTrainers =
                await _context.Trainers.CountAsync();


            model.TotalTrainingHours =
    await _context.SessionFormations
    .SumAsync(s =>
        EF.Functions.DateDiffHour(
            s.DateStart,
            s.DateEnd
        )
    );





            model.TotalBudget =
                await _context.SessionFormations
                .Select(s =>
                    s.TrainerCost +
                    s.HotelCost +
                    s.MealCost +
                    s.KitCost +
                    s.OtherCost
                )
                .SumAsync();






            // ================= STATISTICS =================



            if (model.TotalSessions > 0)
            {
                model.AverageSessionCost =
                    model.TotalBudget / model.TotalSessions;
            }



            if (model.TotalSessions > 0)
            {
                model.AverageParticipants =
                    (double)model.TotalParticipants /
                    model.TotalSessions;
            }





            // Domaine dominant


            var domainStats = await _context.FormationDomains
                .Include(x => x.Domain)
                .GroupBy(x => x.Domain.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();



            model.MostPopularDomain =
                domainStats.FirstOrDefault()?.Name
                ?? "Aucun";





            // ================= DOMAIN CHART =================



            model.DomainLabels =
                domainStats
                .Select(x => x.Name)
                .ToList();



            model.DomainValues =
                domainStats
                .Select(x => x.Count)
                .ToList();







            // ================= EVOLUTION SESSIONS =================



            var monthly = await _context.SessionFormations

                .GroupBy(s => new
                {
                    s.DateStart.Year,
                    s.DateStart.Month
                })

                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })

                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)

                .ToListAsync();





            model.MonthlyLabels =
                monthly.Select(x =>
                    new DateTime(
                        x.Year,
                        x.Month,
                        1)
                    .ToString("MMM")
                )
                .ToList();



            model.MonthlyValues =
                monthly
                .Select(x => x.Count)
                .ToList();








            // ================= LAST FORMATIONS =================



            model.RecentFormations =
                await _context.Formations

                .Include(f => f.Sessions)

                .ThenInclude(s => s.Participants)

                .OrderByDescending(f => f.Id)

                .Take(5)

                .Select(f => new RecentFormationViewModel
                {

                    Title = f.Title,


                    Date =
                    f.Sessions
                    .Select(s => s.DateStart)
                    .FirstOrDefault(),



                    Participants =
                    f.Sessions
                    .SelectMany(s => s.Participants)
                    .Count()


                })

                .ToListAsync();









            // ================= UPCOMING SESSIONS =================



            var today = DateTime.Today;



            model.UpcomingSessions =
                await _context.SessionFormations

                .Include(s => s.Formation)

                .Include(s => s.Participants)

                .Where(s => s.DateStart >= today)

                .OrderBy(s => s.DateStart)

                .Take(5)

                .Select(s => new UpcomingSessionViewModel
                {

                    Formation =
                    s.Formation!.Title,


                    Date =
                    s.DateStart,


                    Location =
                    s.Location,


                    Participants =
                    s.Participants.Count()


                })

                .ToListAsync();







            return View(model);

        }

    
    // ================= ABSENCE JUSTIFICATIONS =================

[HttpGet]
        public async Task<IActionResult> Absences()
        {
            var model = new ResponsableAbsencesPageViewModel();

            var pending = await _context.SessionParticipants

                .Include(p => p.Employee)

                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s.Formation)

                .Where(p => p.AbsenceStatus == "En attente"
                            && p.JustificationFile != null)

                .Select(p => new ResponsableAbsenceItemViewModel
                {
                    ParticipantId = p.Id,

                    EmployeeName =
                        p.Employee!.FirstName + " " +
                        p.Employee.LastName,

                    Matricule =
                        p.Employee.Matricule,

                    FormationTitle =
                        p.SessionFormation!.Formation!.Title,

                    Date =
                        p.SessionFormation.DateStart,

                    JustificationFile =
                        p.JustificationFile,

                    AbsenceStatus =
                        p.AbsenceStatus
                })

                .OrderByDescending(x => x.Date)

                .ToListAsync();


            model.Pending = pending;


            return View(model);
        }
        // ================= ACCEPT ABSENCE =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptAbsence(int participantId)
        {
            var participant = await _context.SessionParticipants
                .Include(p => p.Employee)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s.Formation)
                .FirstOrDefaultAsync(p => p.Id == participantId);


            if (participant == null)
                return NotFound();



            participant.AbsenceStatus = "Justifiée";



            _context.Notifications.Add(new Notification
            {
                EmployeeId = participant.EmployeeId,

                Type = "Absence",

                Message =
                    $"Votre justificatif d'absence pour la formation " +
                    $"« {participant.SessionFormation?.Formation?.Title} » a été accepté.",

                SentDate = DateTime.Now,

                Status = "Non lu"
            });



            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Absences));
        }






        // ================= REFUSE ABSENCE =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectAbsence(int participantId)
        {
            var participant = await _context.SessionParticipants
                .Include(p => p.Employee)
                .Include(p => p.SessionFormation)
                    .ThenInclude(s => s.Formation)
                .FirstOrDefaultAsync(p => p.Id == participantId);



            if (participant == null)
                return NotFound();



            participant.AbsenceStatus = "Non justifiée";



            _context.Notifications.Add(new Notification
            {
                EmployeeId = participant.EmployeeId,

                Type = "Absence",

                Message =
                    $"Votre justificatif d'absence pour la formation " +
                    $"« {participant.SessionFormation?.Formation?.Title} » a été refusé.",

                SentDate = DateTime.Now,

                Status = "Non lu"
            });



            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Absences));
        }
    } }