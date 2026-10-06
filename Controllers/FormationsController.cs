using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models;
using Training_tunisie_telecome.Models.ViewModels;

namespace Training_tunisie_telecome.Controllers
{
    public class FormationsController : Controller
    {
        private readonly AppDbContext _context;


        public FormationsController(AppDbContext context)
        {
            _context = context;
        }



        // ================= INDEX =================

        public async Task<IActionResult> Index()
        {
            var formations = await _context.Formations

                .Include(f => f.FormationDomains)
                    .ThenInclude(fd => fd.Domain)

                .Include(f => f.Sessions)
                    .ThenInclude(s => s.Trainer)

                .Include(f => f.Sessions)
                    .ThenInclude(s => s.Participants)

                .ToListAsync();


            return View(formations);
        }





        // ================= DETAILS =================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();



            var formation = await _context.Formations

                .Include(f => f.FormationDomains)
                    .ThenInclude(fd => fd.Domain)


                .Include(f => f.Sessions)
                    .ThenInclude(s => s.Trainer)


                .Include(f => f.Sessions)
                    .ThenInclude(s => s.Participants)
                        .ThenInclude(p => p.Employee)


                .FirstOrDefaultAsync(f => f.Id == id);



            if (formation == null)
                return NotFound();



            return View(formation);
        }





        // ================= CREATE GET =================

        public async Task<IActionResult> Create()
        {
            var model = new FormationCreateViewModel();

            await LoadLists(model);

            return View(model);
        }






        // ================= CREATE POST =================

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(FormationCreateViewModel model)
        {

            ValidateFormation(model);



            if (ModelState.IsValid)
            {


                Formation formation = new Formation
                {
                    Code = model.Code,

                    Title = model.Title,

                    Module = model.Module,

                    Description = model.Description,

                    Objectives = model.Objectives,

                    Duration = model.Duration
                };





                foreach (var domainId in model.SelectedDomains)
                {
                    formation.FormationDomains.Add(
                        new FormationDomain
                        {
                            DomainId = domainId
                        });
                }





                // IMPORTANT : calculer le nombre réel de participants côté serveur
                // pour que KitTotal et FoodTotal ne restent pas à 0.
                var employees = await _context.Employees
                    .Where(e => model.SelectedDomains.Contains(e.DomainId))
                    .ToListAsync();

                model.NumberOfParticipants = employees.Count;





                SessionFormation session = new SessionFormation
                {
                    TrainerId = model.TrainerId,

                    DateStart = model.DateStart,

                    DateEnd = model.DateEnd,

                    Type = model.Type,

                    Location = model.Location,


                    TrainerCost = model.TrainerCost,

                    HotelNightPrice = model.HotelNightPrice,

                    PenPrice = model.PenPrice,

                    NotebookPrice = model.NotebookPrice,

                    ToteBagPrice = model.ToteBagPrice,

                    RestaurantTicketPrice = model.RestaurantTicketPrice,

                    HotelCost = model.HotelTotal,

                    MealCost = model.FoodTotal,

                    KitCost = model.KitTotal,

                    OtherCost = 0,


                    Status = "À venir"
                };





                formation.Sessions.Add(session);






                // Création automatique des participants selon les domaines sélectionnés

                foreach (var employee in employees)
                {

                    session.Participants.Add(
                        new SessionParticipant
                        {
                            EmployeeId = employee.Id,

                            Confirmed = true
                        });

                }





                _context.Formations.Add(formation);


                await _context.SaveChangesAsync();



                return RedirectToAction(nameof(Index));

            }




            await LoadLists(model);


            return View(model);
        }








        // ================= EDIT GET =================

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
                return NotFound();



            var formation = await _context.Formations

                .Include(f => f.FormationDomains)

                .Include(f => f.Sessions)

                .FirstOrDefaultAsync(f => f.Id == id);




            if (formation == null)
                return NotFound();




            var session = formation.Sessions.FirstOrDefault();




            var model = new FormationCreateViewModel
            {

                Code = formation.Code,

                Title = formation.Title,

                Module = formation.Module,

                Description = formation.Description,

                Objectives = formation.Objectives,

                Duration = formation.Duration,


                SelectedDomains =
                    formation.FormationDomains
                    .Select(fd => fd.DomainId)
                    .ToList(),



                DateStart =
                    session?.DateStart ?? DateTime.Today.AddDays(15),



                DateEnd =
                    session?.DateEnd ?? DateTime.Today.AddDays(17),



                Type =
                    session?.Type ?? "Interne",



                Location =
                    session?.Location ?? "",



                TrainerId =
                    session?.TrainerId ?? 0,



                TrainerCost =
                    session?.TrainerCost ?? 0,

                HotelNightPrice =
                    session?.HotelNightPrice ?? 0,

                PenPrice =
                    session?.PenPrice ?? 0,

                NotebookPrice =
                    session?.NotebookPrice ?? 0,

                ToteBagPrice =
                    session?.ToteBagPrice ?? 0,

                RestaurantTicketPrice =
                    session?.RestaurantTicketPrice ?? 0

            };




            await LoadLists(model);


            return View("Create", model);

        }








        // ================= EDIT POST =================


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(
            int id,
            FormationCreateViewModel model)
        {

            ValidateFormation(model);



            if (ModelState.IsValid)
            {

                var formation = await _context.Formations

                    .Include(f => f.FormationDomains)

                    .Include(f => f.Sessions)

                    .FirstOrDefaultAsync(f => f.Id == id);




                if (formation == null)
                    return NotFound();






                formation.Code = model.Code;

                formation.Title = model.Title;

                formation.Module = model.Module;

                formation.Description = model.Description;

                formation.Objectives = model.Objectives;

                formation.Duration = model.Duration;






                formation.FormationDomains.Clear();





                foreach (var domainId in model.SelectedDomains)
                {

                    formation.FormationDomains.Add(
                        new FormationDomain
                        {
                            DomainId = domainId
                        });

                }






                var session = formation.Sessions.FirstOrDefault();



                if (session == null)
                {
                    session = new SessionFormation();

                    formation.Sessions.Add(session);
                }





                // IMPORTANT : recalculer le nombre réel de participants côté serveur
                // avant d'utiliser KitTotal et FoodTotal.
                model.NumberOfParticipants = await _context.Employees
                    .CountAsync(e => model.SelectedDomains.Contains(e.DomainId));





                session.TrainerId = model.TrainerId;

                session.DateStart = model.DateStart;

                session.DateEnd = model.DateEnd;

                session.Type = model.Type;

                session.Location = model.Location;


                session.TrainerCost = model.TrainerCost;

                session.HotelNightPrice = model.HotelNightPrice;

                session.PenPrice = model.PenPrice;

                session.NotebookPrice = model.NotebookPrice;

                session.ToteBagPrice = model.ToteBagPrice;

                session.RestaurantTicketPrice = model.RestaurantTicketPrice;

                session.HotelCost = model.HotelTotal;

                session.MealCost = model.FoodTotal;

                session.KitCost = model.KitTotal;





                _context.Update(formation);


                await _context.SaveChangesAsync();



                return RedirectToAction(nameof(Index));

            }



            await LoadLists(model);


            return View("Create", model);

        }









        // ================= DELETE =================


        [HttpPost, ActionName("Delete")]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var formation = await _context.Formations.FindAsync(id);



            if (formation != null)
            {

                _context.Formations.Remove(formation);


                await _context.SaveChangesAsync();

            }



            return RedirectToAction(nameof(Index));

        }









        // ================= VALIDATION =================


        private void ValidateFormation(FormationCreateViewModel model)
        {

            if (model.DateStart.Date <= DateTime.Today)
            {
                ModelState.AddModelError(
                    "DateStart",
                    "La date de début doit être dans le futur");
            }



            if (model.DateEnd <= model.DateStart)
            {
                ModelState.AddModelError(
                    "DateEnd",
                    "La date de fin doit être après la date début");
            }



            if (model.SelectedDomains == null ||
                model.SelectedDomains.Count == 0)
            {
                ModelState.AddModelError(
                    "",
                    "Sélectionnez au moins un domaine");
            }

        }








        // ================= LOAD LISTS =================


        private async Task LoadLists(FormationCreateViewModel model)
        {

            model.Domains = await _context.Domains

                .Select(d => new DomainParticipantViewModel
                {
                    Id = d.Id,

                    Name = d.Name,

                    EmployeeCount = d.Employees.Count()

                })

                .ToListAsync();





            model.Trainers =
                await _context.Trainers.ToListAsync();






            model.InternalLocations = new List<string>
            {
                "Salle Formation Tunis",
                "Salle Formation Nabeul",
                "Salle Formation Sousse",
                "Salle Formation Sfax"
            };






            model.Hotels = new List<string>
            {
                "Hôtel Africa Tunis",
                "Golden Tulip Gammarth",
                "Laico Hotel Tunis"
            };

        }

    }
}