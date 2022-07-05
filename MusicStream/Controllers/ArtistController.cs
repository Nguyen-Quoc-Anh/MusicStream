using Microsoft.AspNetCore.Mvc;
using MusicStream.Models;
using System.Collections.Generic;
using static MusicStream.Controllers.Logic.ArtistLogic;
using static MusicStream.Controllers.Logic.AlbumLogic;
namespace MusicStream.Controllers
{
    public class ArtistController : Controller
    {
        public IActionResult Detail(string id)
        {
            Account artist = GetArtistByID(id);
            if (artist == null)
                return NotFound();
            List<Account> artists = GetFeatureArtists();
            List<Album> albums = GetAlbumByArtistId(id);
            ViewData["albums"] = albums;
            ViewData["artists"] = artists;
            return View(artist);
        }
    }
}
