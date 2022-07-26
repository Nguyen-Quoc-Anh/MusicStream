using Microsoft.AspNetCore.Mvc;

namespace MusicStream.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{statusCode}")]
        public IActionResult ErrorPage(int statusCode)
        {
            return View("Index");
        }
    }
}
