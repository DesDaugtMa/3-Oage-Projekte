using Chat.Config;
using Chat.Database;
using Chat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Chat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _appSettings;
        private readonly DatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration appSettings, DatabaseContext context)
        {
            _logger = logger;
            _appSettings = appSettings.GetSection(nameof(AppSettings)).Get<AppSettings>();
            _context = context;
        }

        public IActionResult Index()
        {
            string test = _appSettings.test;
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
