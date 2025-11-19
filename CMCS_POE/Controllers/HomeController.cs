using System.Diagnostics;
using CMCS_POE.Models;
using Microsoft.AspNetCore.Mvc;

namespace CMCS_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (User.IsInRole("HR"))
            {
                return RedirectToAction("Index", "HR");
            }
            else if (User.IsInRole("Lecturer"))
            {
                return RedirectToAction("SubmitClaim", "Lecturer");
            }
            else if (User.IsInRole("Coordinator"))
            {
                return RedirectToAction("AllClaims", "Coordinator");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
