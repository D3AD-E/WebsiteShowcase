using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("403")]
        public IActionResult AccessDenied()
        {
            return View("403");
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View("404");
        }
    }
}
