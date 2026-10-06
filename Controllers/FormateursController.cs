using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models;

namespace Training_tunisie_telecome.Controllers
{
    public class FormateursController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;


        public FormateursController(
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }



        // ================= INDEX =================

        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers
                .Include(t => t.Sessions)
                .ToListAsync();


            return View(trainers);
        }




        // ================= CREATE GET =================

        public IActionResult Create()
        {
            return View();
        }




        // ================= CREATE POST =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Trainer trainer,
            IFormFile? image)
        {

            if (await _context.Trainers
                .AnyAsync(t => t.Matricule == trainer.Matricule))
            {
                ModelState.AddModelError(
                    "Matricule",
                    "Ce matricule existe déjà");
            }



            if (ModelState.IsValid)
            {

                if (image != null)
                {
                    trainer.ImagePath =
                        await UploadImage(image);
                }



                _context.Trainers.Add(trainer);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }



            return View(trainer);
        }




        // ================= DETAILS =================

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
                return NotFound();



            var trainer = await _context.Trainers
                .Include(t => t.Sessions)
                .FirstOrDefaultAsync(t => t.Id == id);



            if (trainer == null)
                return NotFound();



            return View(trainer);
        }




        // ================= EDIT GET =================

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
                return NotFound();



            var trainer =
                await _context.Trainers.FindAsync(id);



            if (trainer == null)
                return NotFound();



            return View(trainer);
        }





        // ================= EDIT POST =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Trainer trainer,
            IFormFile? image)
        {

            if (id != trainer.Id)
                return NotFound();



            if (ModelState.IsValid)
            {

                var oldTrainer =
                    await _context.Trainers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);



                if (image != null)
                {
                    trainer.ImagePath =
                        await UploadImage(image);
                }
                else
                {
                    trainer.ImagePath =
                        oldTrainer!.ImagePath;
                }



                _context.Trainers.Update(trainer);


                await _context.SaveChangesAsync();



                return RedirectToAction(nameof(Index));
            }



            return View(trainer);

        }





        // ================= DELETE =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            var trainer =
                await _context.Trainers.FindAsync(id);



            if (trainer != null)
            {

                _context.Trainers.Remove(trainer);

                await _context.SaveChangesAsync();

            }



            return RedirectToAction(nameof(Index));

        }





        // ================= UPLOAD IMAGE =================

        private async Task<string> UploadImage(IFormFile image)
        {

            string[] allowed =
            {
                ".jpg",
                ".jpeg",
                ".png"
            };



            var extension =
                Path.GetExtension(image.FileName)
                .ToLower();



            if (!allowed.Contains(extension))
            {
                throw new Exception(
                    "Format image non valide");
            }




            if (image.Length > 2 * 1024 * 1024)
            {
                throw new Exception(
                    "Image trop grande");
            }




            string folder =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads/trainers"
                );



            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);




            string fileName =
                Guid.NewGuid()
                + extension;



            string path =
                Path.Combine(
                    folder,
                    fileName);



            using (var stream =
                new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }



            return "/uploads/trainers/" + fileName;

        }

    }
}