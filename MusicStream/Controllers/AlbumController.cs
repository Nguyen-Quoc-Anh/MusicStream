using Microsoft.AspNetCore.Mvc;
using MusicStream.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using static MusicStream.Controllers.Logic.AlbumLogic;
using static MusicStream.Controllers.Logic.TrackLogic;
namespace MusicStream.Controllers
{
    public class AlbumController : Controller
    {
        public IActionResult Detail(string id)
        {
            Album album = GetAlbumDetails(id);
            if (album == null)
            {
                return NotFound();
            }
            Dictionary<Album, List<Artist>> recommendAlbum = GetRandomAlbum(6);
            ViewData["album"] = album;
            ViewData["recommendAlbum"] = recommendAlbum;
            return View(album);
        }

        public IActionResult Index()
        {
            return View();
        }

        public string AddToWishList(string id)
        {
            List<Track> tracks = GetTrackListByAlbum(id);
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return JsonConvert.SerializeObject(tracks, settings);
        }
    }
}
