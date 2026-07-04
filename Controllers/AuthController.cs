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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

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

            HttpContext.Session.SetString("user", user.Matricule);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}