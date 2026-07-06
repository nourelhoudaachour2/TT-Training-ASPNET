using Microsoft.AspNetCore.Mvc;

namespace Training_tunisie_telecome.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}