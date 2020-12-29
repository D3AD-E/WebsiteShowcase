using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Featured()
        {
            return View();
        }

        public IActionResult All()
        {
            return View();
        }
    }
}
