using Microsoft.AspNetCore.Mvc;

namespace MusicStream.Controllers
{
    public class TrackController : Controller
    {
        public IActionResult List()
        {
            return View("List");
        }
    }
}
