using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MusicStream.Models;
using static MusicStream.Controllers.Logic.TrackLogic;
using static MusicStream.Controllers.Logic.ArtistLogic;
using static MusicStream.Controllers.Logic.AlbumLogic;
using static MusicStream.Controllers.Logic.GenreLogic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MusicStreamingver1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Dictionary<Track, List<Account>> tracks = GetFeatureTrack();
            Dictionary<Track, List<Account>> popularTracks = GetMostPopularTrack();
            Dictionary<Album, List<Account>> albums = GetFeatureAlbum();
            List<Account> artists = GetFeatureArtists();
            ViewData["tracks"] = tracks;
            ViewData["popularTracks"] = popularTracks;
            ViewData["artists"] = artists;
            ViewData["albums"] = albums;
            return View("Index");
        }

        public IActionResult Search(string key, string sortby, string[] genres, string[] artists)
        {
            List<Account> artist = GetAllArtist();
            ViewData["artists"] = artist;
            ViewData["genres"] = GetAllGenresAsMultiSelectList();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
