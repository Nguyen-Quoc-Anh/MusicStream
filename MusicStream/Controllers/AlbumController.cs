using Microsoft.AspNetCore.Mvc;
using MusicStream.Models;
using System.Collections.Generic;
using static MusicStream.Controllers.Logic.AlbumLogic;
using static MusicStream.Controllers.Logic.TrackLogic;
namespace MusicStream.Controllers
{
    public class AlbumController : Controller
    {
        public IActionResult Detail(string id)
        {
            Dictionary<Album, List<Account>> album = GetAlbumDetails(id);
            if (album == null)
            {
                return NotFound();
            }
            Dictionary<Track, List<Account>> tracks = GetTrackByAlbum(id);
            Dictionary<Album, List<Account>> recommendAlbum = GetRandomAlbum(6);
            ViewData["album"] = album;
            ViewData["tracks"] = tracks;
            ViewData["recommendAlbum"] = recommendAlbum;
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
