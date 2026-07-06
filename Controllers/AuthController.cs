using Microsoft.AspNetCore.Mvc;
using Training_tunisie_telecome.Data;
using System.Linq;

namespace Training_tunisie_telecome.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string matricule, string password)
        {
            var user = _context.Responsables
                .FirstOrDefault(x => x.Matricule == matricule);

            if (user == null)
            {
                ViewBag.Error = "Matricule incorrect";
                return View();
            }

            if (user.PasswordHash != password)
            {
                ViewBag.Error = "Mot de passe incorrect";
                return View();
            }

            // session login
            HttpContext.Session.SetString("user", user.Matricule);

            // redirect dashboard
            return RedirectToAction("Index", "Dashboard");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}