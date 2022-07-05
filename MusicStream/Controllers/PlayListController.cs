using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MusicStream.Controllers
{
    [Authorize]
    public class PlayListController : Controller
    {
        public string Index()
        {
            return "test";
        }
    }
}
