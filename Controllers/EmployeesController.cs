using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models;

namespace Training_tunisie_telecome.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Employee> _passwordHasher = new();

        private const string DefaultEmployeePassword = "TT@2026";


        public EmployeesController(
            AppDbContext context,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }



        // ================= INDEX =================

        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Domain)
                .ToListAsync();

            return View(employees);
        }



        // ================= CREATE GET =================

        public IActionResult Create()
        {
            LoadSelectLists();

            return View();
        }



        // ================= CREATE POST =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Employee employee,
            IFormFile? image,
            bool createAuthenticationAccount = false)
        {

            if (await _context.Employees
                .AnyAsync(e => e.Matricule == employee.Matricule))
            {
                ModelState.AddModelError(
                    "Matricule",
                    "Ce matricule existe déjà");
            }


            if (createAuthenticationAccount &&
                await _context.Employees.AnyAsync(e =>
                    e.AuthenticationEnabled &&
                    e.Email == employee.Email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Un compte d'authentification existe déjà avec cet email");
            }


            if (ModelState.IsValid)
            {

                if (image != null)
                {
                    employee.ImagePath =
                        await UploadImage(image);
                }


                if (createAuthenticationAccount)
                {
                    employee.AuthenticationEnabled = true;
                    employee.MustChangePassword = true;

                    employee.PasswordHash =
                        _passwordHasher.HashPassword(
                            employee,
                            DefaultEmployeePassword);
                }
                else
                {
                    employee.AuthenticationEnabled = false;
                    employee.MustChangePassword = false;
                    employee.PasswordHash = null;
                }


                _context.Employees.Add(employee);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }


            ViewBag.CreateAuthenticationAccount =
                createAuthenticationAccount;

            LoadSelectLists();

            return View(employee);
        }





        // ================= DETAILS =================

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
                return NotFound();


            var employee = await _context.Employees
                .Include(e => e.Domain)
                .FirstOrDefaultAsync(e => e.Id == id);


            if (employee == null)
                return NotFound();


            return View(employee);
        }





        // ================= EDIT GET =================

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
                return NotFound();


            var employee = await _context.Employees
                .FindAsync(id);


            if (employee == null)
                return NotFound();


            LoadSelectLists();


            return View(employee);
        }





        // ================= EDIT POST =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Employee employee,
            IFormFile? image)
        {

            if (id != employee.Id)
                return NotFound();


            var oldEmployee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);


            if (oldEmployee == null)
                return NotFound();


            if (oldEmployee.AuthenticationEnabled &&
                await _context.Employees.AnyAsync(e =>
                    e.Id != id &&
                    e.AuthenticationEnabled &&
                    e.Email == employee.Email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Un compte d'authentification existe déjà avec cet email");
            }



            if (ModelState.IsValid)
            {

                if (image != null)
                {
                    employee.ImagePath =
                        await UploadImage(image);
                }
                else
                {
                    employee.ImagePath =
                        oldEmployee.ImagePath;
                }


                // Ne jamais écraser le compte d'authentification
                // lors d'une simple modification de l'employé.
                employee.AuthenticationEnabled =
                    oldEmployee.AuthenticationEnabled;

                employee.PasswordHash =
                    oldEmployee.PasswordHash;

                employee.MustChangePassword =
                    oldEmployee.MustChangePassword;


                _context.Employees.Update(employee);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }


            LoadSelectLists();

            return View(employee);
        }





        // ================= DELETE =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            var employee =
                await _context.Employees.FindAsync(id);


            if (employee != null)
            {

                _context.Employees.Remove(employee);

                await _context.SaveChangesAsync();

            }


            return RedirectToAction(nameof(Index));
        }





        // ================= LOAD SELECT LISTS =================

        private void LoadSelectLists()
        {

            ViewBag.Domains =
                new SelectList(
                    _context.Domains.ToList(),
                    "Id",
                    "Name"
                );


            ViewBag.Grades =
                new SelectList(
                    ReadJsonFile("GradesFile")
                );


            ViewBag.Residences =
                new SelectList(
                    ReadJsonFile("ResidencesFile")
                );

        }





        // ================= READ JSON =================

        private List<string> ReadJsonFile(string key)
        {

            var path =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    _configuration[$"EmployeeData:{key}"]!
                );


            if (!System.IO.File.Exists(path))
                return new List<string>();


            var json =
                System.IO.File.ReadAllText(path);


            return JsonSerializer
                .Deserialize<List<string>>(json)
                ?? new List<string>();
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
                    "uploads/employees"
                );


            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);



            string fileName =
                Guid.NewGuid()
                + extension;



            string path =
                Path.Combine(
                    folder,
                    fileName
                );


            using (var stream =
                new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }



            return "/uploads/employees/" + fileName;
        }
    }
}