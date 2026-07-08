using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models;

namespace Training_tunisie_telecome.Controllers
{
    public class ServicesController : Controller
    {

        private readonly AppDbContext _context;


        public ServicesController(AppDbContext context)
        {
            _context = context;
        }



        // GET: Services
        public async Task<IActionResult> Index()
        {

            var services = await _context.Services
                .Include(s => s.Employees)
                .ToListAsync();


            return View(services);

        }





        // GET: Create
        public IActionResult Create()
        {
            return View();
        }




        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {


            if (ModelState.IsValid)
            {

                _context.Services.Add(service);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));

            }



            return View(service);

        }







        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {


            if (id == null)
                return NotFound();



            var service = await _context.Services.FindAsync(id);



            if (service == null)
                return NotFound();



            return View(service);

        }







        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {


            if (id != service.Id)
                return NotFound();



            if (ModelState.IsValid)
            {

                _context.Services.Update(service);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));

            }



            return View(service);

        }







        // POST DELETE FROM POPUP

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {


            var service = await _context.Services.FindAsync(id);



            if (service != null)
            {

                _context.Services.Remove(service);

                await _context.SaveChangesAsync();

            }



            return RedirectToAction(nameof(Index));


        }
        // GET: Services/Details/5

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
                return NotFound();


            var service = await _context.Services
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(s => s.Id == id);



            if (service == null)
                return NotFound();



            return View(service);

        }


    }

}