using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Website.Data;
using Website.Models;
using Website.Models.ViewModels;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ContactMe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe([Bind("Name,Email,Message")] ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(contactModel);
                    await _context.SaveChangesAsync();
                    TempData["UserMessage"] = new MessageViewModel() { CssClassName = "alert alert-success", Title = "", Message = "Thank you for contacting me!" };
                    return View();
                }
                catch (Exception e)
                {
                    
                }
            }
            TempData["UserMessage"] = new MessageViewModel() { CssClassName = "alert alert-danger", Title = "Error!", Message = "Something went wrong" };
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Cv()
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
