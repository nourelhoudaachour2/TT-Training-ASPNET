using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Models;

namespace Training_tunisie_telecome.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Employee> _employeePasswordHasher = new();

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // ================= LOGIN GET =================
        [HttpGet]
        public IActionResult Login()
        {
            var role = HttpContext.Session.GetString("role");

            if (role == "Responsable")
                return RedirectToAction("Index", "Dashboard");

            if (role == "Employee")
                return RedirectToAction("Index", "EmployeeDashboard");

            return View();
        }

        // ================= LOGIN POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string identifiant, string password)
        {
            identifiant = (identifiant ?? string.Empty).Trim();
            password ??= string.Empty;

            if (string.IsNullOrWhiteSpace(identifiant) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Veuillez saisir votre identifiant et votre mot de passe.";
                ViewBag.Identifiant = identifiant;
                return View();
            }

            // -------------------------------------------------
            // 1) Responsable RH : connexion par matricule
            // On conserve la logique actuelle du projet.
            // -------------------------------------------------
            var responsable = await _context.Responsables
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Matricule == identifiant);

            if (responsable != null && responsable.PasswordHash == password)
            {
                HttpContext.Session.Clear();

                HttpContext.Session.SetString("user", responsable.Matricule);
                HttpContext.Session.SetString("role", "Responsable");
                HttpContext.Session.SetString("displayName", "Responsable RH");

                return RedirectToAction("Index", "Dashboard");
            }

            // -------------------------------------------------
            // 2) Employé : connexion par email d'authentification
            // -------------------------------------------------
            var normalizedIdentifier = identifiant.ToLower();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.AuthenticationEnabled &&
                    e.Email.ToLower() == normalizedIdentifier);

            if (employee == null || string.IsNullOrWhiteSpace(employee.PasswordHash))
            {
                ViewBag.Error = "Identifiant ou mot de passe incorrect.";
                ViewBag.Identifiant = identifiant;
                return View();
            }

            var verification = _employeePasswordHasher.VerifyHashedPassword(
                employee,
                employee.PasswordHash,
                password);

            if (verification == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Identifiant ou mot de passe incorrect.";
                ViewBag.Identifiant = identifiant;
                return View();
            }

            HttpContext.Session.Clear();

            HttpContext.Session.SetString("user", employee.Email);
            HttpContext.Session.SetString("role", "Employee");
            HttpContext.Session.SetInt32("employeeId", employee.Id);
            HttpContext.Session.SetString(
                "displayName",
                $"{employee.FirstName} {employee.LastName}");

            if (!string.IsNullOrWhiteSpace(employee.ImagePath))
            {
                HttpContext.Session.SetString("employeeImage", employee.ImagePath);
            }

            if (employee.MustChangePassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            return RedirectToAction("Index", "EmployeeDashboard");
        }

        // ================= CHANGE PASSWORD GET =================
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("role") != "Employee" ||
                HttpContext.Session.GetInt32("employeeId") == null)
            {
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        // ================= CHANGE PASSWORD POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            var employeeId = HttpContext.Session.GetInt32("employeeId");

            if (HttpContext.Session.GetString("role") != "Employee" ||
                employeeId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId.Value);

            if (employee == null || string.IsNullOrWhiteSpace(employee.PasswordHash))
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            if (string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "Tous les champs sont obligatoires.";
                return View();
            }

            var currentVerification = _employeePasswordHasher.VerifyHashedPassword(
                employee,
                employee.PasswordHash,
                currentPassword);

            if (currentVerification == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Le mot de passe actuel est incorrect.";
                return View();
            }

            if (newPassword.Length < 8)
            {
                ViewBag.Error = "Le nouveau mot de passe doit contenir au moins 8 caractères.";
                return View();
            }

            if (!newPassword.Any(char.IsUpper))
            {
                ViewBag.Error = "Le nouveau mot de passe doit contenir au moins une lettre majuscule.";
                return View();
            }

            if (!newPassword.Any(char.IsLower))
            {
                ViewBag.Error = "Le nouveau mot de passe doit contenir au moins une lettre minuscule.";
                return View();
            }

            if (!newPassword.Any(char.IsDigit))
            {
                ViewBag.Error = "Le nouveau mot de passe doit contenir au moins un chiffre.";
                return View();
            }

            if (!newPassword.Any(c => !char.IsLetterOrDigit(c)))
            {
                ViewBag.Error = "Le nouveau mot de passe doit contenir au moins un caractère spécial.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "La confirmation du mot de passe est incorrecte.";
                return View();
            }

            if (newPassword == currentPassword)
            {
                ViewBag.Error = "Le nouveau mot de passe doit être différent du mot de passe initial.";
                return View();
            }

            employee.PasswordHash = _employeePasswordHasher.HashPassword(
                employee,
                newPassword);

            employee.MustChangePassword = false;

            await _context.SaveChangesAsync();

            TempData["PasswordChanged"] = "Mot de passe modifié avec succès.";

            return RedirectToAction("Index", "EmployeeDashboard");
        }

        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}