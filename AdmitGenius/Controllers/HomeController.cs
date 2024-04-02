using AdmitGenius.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdmitGenius.Controllers
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
            return View();
        }

        public IActionResult AboutUs()
        {
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

        public IActionResult ExploreUniversitiesCanada()
        {
            // Redirect to StudentHomePage with a filter for Canada
            return RedirectToAction("StudentHomePage","StudentHome", new { countryFilter = "CANADA" });
        }

        public IActionResult ExploreUniversitiesUS()
        {
            // Redirect to StudentHomePage with a filter for the US
            return RedirectToAction("StudentHomePage", "StudentHome", new { countryFilter = "US" });
        }

        public IActionResult ExploreUniversitiesUK()
        {
            // Redirect to StudentHomePage with a filter for the UK
            return RedirectToAction("StudentHomePage", "StudentHome", new { countryFilter = "UK" });
        }
    }
}