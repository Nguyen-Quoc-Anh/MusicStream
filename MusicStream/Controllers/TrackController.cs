using Microsoft.AspNetCore.Mvc;
using MusicStream.Controllers.Logic;
using MusicStream.Logic;
using MusicStream.Models;
using Newtonsoft.Json;

namespace MusicStream.Controllers
{
    public class TrackController : Controller
    {
        public IActionResult List()
        {
            return View("List");
        }
        public string LikeTrack(string Id)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || TrackLogic.CheckAccountLikedTrack(account.AccountId, Id))
            {
                return JsonConvert.SerializeObject(new Action("like", false));
            }
            if (TrackLogic.LikeTrack(account.AccountId, Id))
            {
                return JsonConvert.SerializeObject(new Action("like", true));
            }
            return JsonConvert.SerializeObject(new Action("like", false));
        }

        [HttpPut]
        public void IncreaseListens(string Id)
        {
            if (TrackLogic.CheckTrackExistById(Id))
            {
                TrackLogic.IncreaseListens(Id);
            }
        }
    }
}
