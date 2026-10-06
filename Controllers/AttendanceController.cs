using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace Training_tunisie_telecome.Controllers
{

    public class AttendanceController : Controller
    {

        private readonly AppDbContext _context;


        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }





        // ================= INDEX =================


        public async Task<IActionResult> Index()
        {

            var today = DateTime.Today;


            var sessions = await _context.SessionFormations

                .Include(s => s.Formation)

                .Include(s => s.Trainer)

                .Include(s => s.Participants)

                    .ThenInclude(p => p.Employee)

                        .ThenInclude(e => e.Domain)


                .Where(s => s.DateStart.Date <= today)


                .OrderByDescending(s => s.DateStart)


                .ToListAsync();



            return View(sessions);

        }









        // ================= TAKE =================


        public async Task<IActionResult> Take(int id)
        {



            var session = await _context.SessionFormations


                .Include(s => s.Formation)


                .Include(s => s.Trainer)



                .Include(s => s.Participants)

                    .ThenInclude(p => p.Employee)

                        .ThenInclude(e => e.Domain)



                .Include(s => s.Participants)

                    .ThenInclude(p => p.ReplacementEmployee)



                .Include(s => s.Participants)

                    .ThenInclude(p => p.Attendances)



                .FirstOrDefaultAsync(s => s.Id == id);




            if (session == null)
                return NotFound();





            ViewBag.Employees = await _context.Employees
                .OrderBy(e => e.FirstName)
                .ToListAsync();




            return View(session);

        }












        // ================= SAVE =================



        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Save(

            int sessionId,

            Dictionary<int, string> statuses,

            Dictionary<int, int?> replacements

            )
        {



            foreach (var item in statuses)
            {


                var participant = await _context.SessionParticipants

     .Include(p => p.Attendances)

     .Include(p => p.SessionFormation)
         .ThenInclude(s => s.Formation)

     .FirstOrDefaultAsync(
         p => p.Id == item.Key
     );




                if (participant == null)
                    continue;





                string status = item.Value;
                // ===============================
                // Notification absence employee
                // ===============================

                if (status == "Absent")
                {
                    var alreadyNotifiedToday = await _context.Notifications
                        .AnyAsync(n =>
                            n.EmployeeId == participant.EmployeeId &&
                            n.Type == "Absence" &&
                            n.SentDate.Date == DateTime.Today &&
                            n.Message.Contains(
                                participant.SessionFormation.Formation.Title
                            )
                        );


                    if (!alreadyNotifiedToday)
                    {
                        var notification = new Notification
                        {
                            EmployeeId = participant.EmployeeId,

                            Type = "Absence",

                            Message =
                            $"Vous avez été marqué absent à la formation " +
                            $"{participant.SessionFormation?.Formation?.Title}. " +
                            $"Vous disposez de 72 heures pour envoyer un justificatif PDF.",

                            SentDate = DateTime.Now,

                            Status = "Non lu"
                        };


                        _context.Notifications.Add(notification);
                    }
                }



                // Mise à jour actuelle


                participant.Present =
                    status == "Présent";




                participant.AbsenceStatus =
                    status == "Absent"
                    ?
                    "Non justifiée"
                    :
                    status == "Excusé"
                    ?
                    "Justifiée"
                    :
                    null;







                // remplacement


                if (replacements != null &&

                   replacements.TryGetValue(
                        item.Key,
                        out var replacementId))

                {

                    participant.ReplacementEmployeeId =
                        replacementId;

                }







                // vérifier si présence déjà enregistrée aujourd'hui


                var oldAttendance = await _context.Attendances

                    .FirstOrDefaultAsync(a =>

                        a.SessionParticipantId == participant.Id

                        &&

                        a.Date.Date == DateTime.Today

                    );






                if (oldAttendance != null)
                {


                    // modification au lieu de créer doublon


                    oldAttendance.Status = status;

                    oldAttendance.CreatedAt =
                        DateTime.Now;


                    oldAttendance.CreatedBy =
                        "Responsable RH";


                }

                else
                {


                    var attendance = new Attendance
                    {


                        SessionParticipantId =
                            participant.Id,


                        Date =
                            DateTime.Today,


                        Status =
                            status,


                        CreatedAt =
                            DateTime.Now,


                        CreatedBy =
                            "Responsable RH"

                    };



                    _context.Attendances.Add(attendance);

                }

            }






            await _context.SaveChangesAsync();





            return RedirectToAction(
                nameof(Take),
                new { id = sessionId }
            );

        }














        // ================= PDF =================


        public async Task<IActionResult> ExportPdf(int id)
        {



            var session = await _context.SessionFormations

                .Include(s => s.Formation)

                .Include(s => s.Participants)

                    .ThenInclude(p => p.Employee)

                        .ThenInclude(e => e.Domain)


                .FirstOrDefaultAsync(
                    s => s.Id == id
                );




            if (session == null)
                return NotFound();





            var pdf = Document.Create(container =>
            {


                container.Page(page =>
                {

                    page.Size(PageSizes.A4);

                    page.Margin(30);



                    page.Header()

                    .Text(
                    "Tunisie Telecom - Liste de présence"
                    )

                    .Bold()
                    .FontSize(18);





                    page.Content()

                    .Column(column =>
                    {


                        column.Item()

                        .Text(
                        $"Formation : {session.Formation?.Title}"
                        );



                        column.Item()

                        .Text(
                        $"Date : {session.DateStart:dd/MM/yyyy}"
                        );



                        column.Item()

                        .Table(table =>
                        {


                            table.ColumnsDefinition(c =>
                            {

                                c.RelativeColumn();

                                c.RelativeColumn();

                                c.RelativeColumn();

                                c.RelativeColumn();


                            });



                            table.Header(h =>
                            {

                                h.Cell().Text("Employé");

                                h.Cell().Text("Matricule");

                                h.Cell().Text("Domaine");

                                h.Cell().Text("Statut");


                            });




                            foreach (var p in session.Participants)
                            {


                                var status =
                                    p.Attendances
                                    .OrderByDescending(a => a.Date)
                                    .FirstOrDefault()
                                    ?.Status
                                    ??
                                    "Absent";



                                table.Cell()

                                .Text(
                                $"{p.Employee?.FirstName} {p.Employee?.LastName}"
                                );



                                table.Cell()

                                .Text(
                                p.Employee?.Matricule ?? ""
                                );



                                table.Cell()

                                .Text(
                                p.Employee?.Domain?.Name ?? ""
                                );



                                table.Cell()

                                .Text(status);



                            }



                        });


                    });





                    page.Footer()

                    .AlignCenter()

                    .Text(
                    $"Généré le {DateTime.Now}"
                    );


                });


            })
            .GeneratePdf();





            return File(

                pdf,

                "application/pdf",

                $"Presence_{session.Formation?.Title}.pdf"

            );



        }



    }

}