using Microsoft.AspNetCore.Mvc;

namespace MusicStream.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return View("Error");
        }
    }
}
