using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models.ViewModels;

namespace Training_tunisie_telecome.Controllers
{
    public class ReportingController : Controller
    {
        private readonly AppDbContext _context;

        public ReportingController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // PAGE PRINCIPALE REPORTING
        // =========================================================

        public IActionResult Index()
        {
            return View();
        }


        // =========================================================
        // RAPPORT FORMATIONS
        // =========================================================

        public async Task<IActionResult> Formations()
        {
            var formations = await _context.Formations
                .Include(f => f.FormationDomains)
                    .ThenInclude(fd => fd.Domain)

                .Include(f => f.Sessions)
                    .ThenInclude(s => s.Participants)

                .OrderByDescending(f => f.Id)
                .ToListAsync();

            return View(formations);
        }


        // =========================================================
        // RAPPORT EMPLOYES
        // =========================================================

        public async Task<IActionResult> Employee()
        {
            var employees = await _context.Employees
                .Include(e => e.Domain)

                .Include(e => e.SessionParticipants)
                    .ThenInclude(sp => sp.SessionFormation)
                        .ThenInclude(sf => sf.Formation)

                .Include(e => e.SessionParticipants)
                    .ThenInclude(sp => sp.Attendances)

                .ToListAsync();

            var report = employees.Select(e =>
            {
                var participants = e.SessionParticipants
                    ?? new List<Models.SessionParticipant>();

                var employeeFormations = participants
                    .Where(sp => sp.SessionFormation != null)
                    .ToList();

                // Nombre de formations différentes
                var formationCount = employeeFormations
                    .Where(sp => sp.SessionFormation != null)
                    .Select(sp => sp.SessionFormation!.FormationId)
                    .Distinct()
                    .Count();

                // Total heures de formation
                var trainingHours = employeeFormations
                    .Where(sp =>
                        sp.SessionFormation != null &&
                        sp.SessionFormation.Formation != null)
                    .Sum(sp => sp.SessionFormation!.Formation!.Duration);

                // Présences
                var attendances = participants
                    .SelectMany(sp =>
                        sp.Attendances ?? new List<Models.Attendance>())
                    .ToList();

                var presentCount = attendances
                    .Count(a => a.Status == "Présent");

                var attendanceRate = attendances.Count > 0
                    ? (double)presentCount / attendances.Count * 100
                    : 0;

                // Dernière formation
                var lastFormation = employeeFormations
                    .Where(sp => sp.SessionFormation != null)
                    .OrderByDescending(sp => sp.SessionFormation!.DateStart)
                    .Select(sp => (DateTime?)sp.SessionFormation!.DateStart)
                    .FirstOrDefault();

                return new Models.ViewModels.EmployeeReportViewModel
                {
                    Matricule = e.Matricule,
                    FullName = $"{e.FirstName} {e.LastName}",
                    Domain = e.Domain?.Name ?? "",
                    FormationCount = formationCount,
                    TrainingHours = trainingHours,
                    AttendanceRate = attendanceRate,
                    LastFormation = lastFormation
                };

            }).ToList();

            return View(report);
        }


        // =========================================================
        // RAPPORT BUDGET
        // =========================================================

        public async Task<IActionResult> Budget()
        {
            var sessions = await _context.SessionFormations
                .Include(s => s.Formation)
                .Include(s => s.Trainer)
                .Include(s => s.Participants)
                .OrderByDescending(s => s.DateStart)
                .ToListAsync();

            var report = sessions.Select(s => new Models.ViewModels.BudgetReportViewModel
            {
                Formation = s.Formation?.Title ?? "",
                TrainerCost = s.TrainerCost,
                HotelCost = s.HotelCost,
                MealCost = s.MealCost,
                KitCost = s.KitCost,
                OtherCost = s.OtherCost,
                Total = s.TotalCost,
                ParticipantCount = s.Participants?.Count ?? 0,
                DateStart = s.DateStart,
                DateEnd = s.DateEnd,
                TrainerName = s.Trainer != null ? $"{s.Trainer.FirstName} {s.Trainer.LastName}" : "",
                Location = s.Location,
                Type = s.Type
            }).ToList();

            return View(report);
        }


        // =========================================================
        // RAPPORT PRESENCE
        // =========================================================

        public async Task<IActionResult> Attendance()
        {
            var sessions = await _context.SessionFormations

                .Include(s => s.Formation)

                .Include(s => s.Participants)
                    .ThenInclude(p => p.Employee)

                .Include(s => s.Participants)
                    .ThenInclude(p => p.Attendances)

                .OrderByDescending(s => s.DateStart)

                .ToListAsync();

            return View(sessions);
        }


        // =========================================================
        // ASSISTANT LOCAL - GET
        // =========================================================

        [HttpGet]
        public IActionResult Assistant()
        {
            return View(new AiRequestViewModel());
        }


        // =========================================================
        // ASSISTANT LOCAL - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assistant(AiRequestViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Question))
            {
                ModelState.AddModelError(
                    nameof(model.Question),
                    "Veuillez saisir une question."
                );

                return View(model);
            }

            var question = NormalizeText(model.Question);

            var formations = await _context.Formations
                .AsNoTracking()
                .Include(f => f.FormationDomains)
                    .ThenInclude(fd => fd.Domain)
                .Include(f => f.Sessions)
                    .ThenInclude(s => s.Participants)
                        .ThenInclude(p => p.Attendances)
                .ToListAsync();

            var sessions = formations
                .SelectMany(f => f.Sessions)
                .ToList();

            var employees = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Domain)
                .ToListAsync();


            // =====================================================
            // FORMATIONS LES PLUS SUIVIES
            // =====================================================

            if ((question.Contains("formation") || question.Contains("formations")) &&
                (question.Contains("plus suiv") ||
                 question.Contains("populaire") ||
                 question.Contains("plus populaire")))
            {
                var ranking = formations
                    .Select(f => new
                    {
                        f.Title,
                        ParticipantCount = f.Sessions
                            .SelectMany(s => s.Participants)
                            .Select(p => p.EmployeeId)
                            .Distinct()
                            .Count()
                    })
                    .OrderByDescending(x => x.ParticipantCount)
                    .ThenBy(x => x.Title)
                    .Take(10)
                    .ToList();

                if (ranking.Count == 0)
                {
                    model.Answer = "Aucune formation n'est disponible.";
                    return View(model);
                }

                var sb = new StringBuilder();

                sb.AppendLine("Formations les plus suivies :");
                sb.AppendLine();

                for (int i = 0; i < ranking.Count; i++)
                {
                    sb.AppendLine(
                        $"{i + 1}. {ranking[i].Title} : {ranking[i].ParticipantCount} participant(s)"
                    );
                }

                model.Answer = sb.ToString();

                return View(model);
            }


            // =====================================================
            // TAUX DE PARTICIPATION / PRESENCE
            // =====================================================

            if (question.Contains("taux") &&
                (question.Contains("participation") ||
                 question.Contains("presence") ||
                 question.Contains("assiduite")))
            {
                var attendances = sessions
                    .SelectMany(s => s.Participants)
                    .SelectMany(p =>
                        p.Attendances ?? new List<Models.Attendance>())
                    .ToList();

                var presentCount = attendances.Count(a =>
                    NormalizeText(a.Status) == "present");

                var absentCount = attendances.Count(a =>
                    NormalizeText(a.Status) == "absent");

                var excusedCount = attendances.Count(a =>
                    NormalizeText(a.Status) == "excuse");

                var total = attendances.Count;

                var rate = total > 0
                    ? (double)presentCount / total * 100
                    : 0;

                model.Answer =
                    $"Taux de présence global : {rate:F2} %\n\n" +
                    $"Présences : {presentCount}\n" +
                    $"Absences : {absentCount}\n" +
                    $"Excusés : {excusedCount}\n" +
                    $"Total des enregistrements de présence : {total}";

                return View(model);
            }


            // =====================================================
            // COUT TOTAL / BUDGET
            // =====================================================

            if (question.Contains("cout") ||
                question.Contains("budget") ||
                question.Contains("depense"))
            {
                var trainerCost = sessions.Sum(s => s.TrainerCost);
                var hotelCost = sessions.Sum(s => s.HotelCost);
                var mealCost = sessions.Sum(s => s.MealCost);
                var kitCost = sessions.Sum(s => s.KitCost);
                var otherCost = sessions.Sum(s => s.OtherCost);

                var total =
                    trainerCost +
                    hotelCost +
                    mealCost +
                    kitCost +
                    otherCost;

                model.Answer =
                    $"Coût total des formations : {total:N3} DT\n\n" +
                    $"Formateurs : {trainerCost:N3} DT\n" +
                    $"Hébergement : {hotelCost:N3} DT\n" +
                    $"Repas : {mealCost:N3} DT\n" +
                    $"Kits : {kitCost:N3} DT\n" +
                    $"Autres coûts : {otherCost:N3} DT";

                return View(model);
            }


            // =====================================================
            // DOMAINES LES PLUS POPULAIRES
            // =====================================================

            if (question.Contains("domaine") &&
                (question.Contains("populaire") ||
                 question.Contains("plus suivi") ||
                 question.Contains("plus demande")))
            {
                var domainRanking = formations
                    .SelectMany(f => f.FormationDomains)
                    .Where(fd => fd.Domain != null)
                    .GroupBy(fd => new
                    {
                        fd.DomainId,
                        Name = fd.Domain!.Name
                    })
                    .Select(g => new
                    {
                        g.Key.Name,
                        FormationCount = g
                            .Select(fd => fd.FormationId)
                            .Distinct()
                            .Count()
                    })
                    .OrderByDescending(x => x.FormationCount)
                    .ThenBy(x => x.Name)
                    .ToList();

                if (domainRanking.Count == 0)
                {
                    model.Answer =
                        "Aucune donnée de domaine n'est disponible.";

                    return View(model);
                }

                var sb = new StringBuilder();

                sb.AppendLine("Domaines de formation les plus représentés :");
                sb.AppendLine();

                for (int i = 0; i < domainRanking.Count; i++)
                {
                    sb.AppendLine(
                        $"{i + 1}. {domainRanking[i].Name} : {domainRanking[i].FormationCount} formation(s)"
                    );
                }

                model.Answer = sb.ToString();

                return View(model);
            }


            // =====================================================
            // NOMBRE TOTAL DE FORMATIONS
            // =====================================================

            if (question.Contains("combien") &&
                question.Contains("formation"))
            {
                model.Answer =
                    $"Nombre total de formations : {formations.Count}\n" +
                    $"Nombre total de sessions : {sessions.Count}";

                return View(model);
            }


            // =====================================================
            // NOMBRE TOTAL D'EMPLOYES
            // =====================================================

            if (question.Contains("combien") &&
                question.Contains("employ"))
            {
                model.Answer =
                    $"Nombre total d'employés : {employees.Count}";

                return View(model);
            }


            // =====================================================
            // NOMBRE TOTAL DE SESSIONS
            // =====================================================

            if (question.Contains("combien") &&
                question.Contains("session"))
            {
                model.Answer =
                    $"Nombre total de sessions de formation : {sessions.Count}";

                return View(model);
            }


            // =====================================================
            // TOTAL HEURES DE FORMATION
            // =====================================================

            if (question.Contains("heure") ||
                question.Contains("duree"))
            {
                var totalHours = formations.Sum(f => f.Duration);

                model.Answer =
                    $"Durée totale des formations enregistrées : {totalHours} heure(s).";

                return View(model);
            }


            // =====================================================
            // FORMATIONS INTERNES / EXTERNES
            // =====================================================

            if (question.Contains("interne") ||
                question.Contains("externe"))
            {
                var internalCount = sessions.Count(s =>
                    NormalizeText(s.Type) == "interne");

                var externalCount = sessions.Count(s =>
                    NormalizeText(s.Type) == "externe");

                model.Answer =
                    $"Sessions internes : {internalCount}\n" +
                    $"Sessions externes : {externalCount}";

                return View(model);
            }


            // =====================================================
            // RESUME GENERAL
            // =====================================================

            if (question.Contains("resume") ||
                question.Contains("synthese") ||
                question.Contains("statistique") ||
                question.Contains("rapport general"))
            {
                var totalBudget = sessions.Sum(s => s.TotalCost);

                var totalParticipants = sessions
                    .SelectMany(s => s.Participants)
                    .Count();

                model.Answer =
                    "Résumé général des formations :\n\n" +
                    $"Formations : {formations.Count}\n" +
                    $"Sessions : {sessions.Count}\n" +
                    $"Employés : {employees.Count}\n" +
                    $"Participations enregistrées : {totalParticipants}\n" +
                    $"Budget total : {totalBudget:N3} DT";

                return View(model);
            }


            // =====================================================
            // QUESTION NON RECONNUE
            // =====================================================

            model.Answer =
                "Je peux actuellement répondre aux questions suivantes :\n\n" +
                "• Quelles sont les formations les plus suivies ?\n" +
                "• Quel est le taux de participation ?\n" +
                "• Quel est le coût total des formations ?\n" +
                "• Quels sont les domaines les plus populaires ?\n" +
                "• Combien de formations, sessions ou employés existe-t-il ?\n" +
                "• Quelle est la durée totale des formations ?\n" +
                "• Combien de sessions internes et externes ?\n" +
                "• Donne-moi un résumé général.";

            return View(model);
        }


        // =========================================================
        // NORMALISATION TEXTE
        // =========================================================

        private static string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalized = text
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var category =
                    CharUnicodeInfo.GetUnicodeCategory(c);

                if (category != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }


        // =========================================================
        // TOUS LES RAPPORTS
        // =========================================================

        public IActionResult AllReports()
        {
            return View();
        }
    }
}