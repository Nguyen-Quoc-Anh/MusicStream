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
using X.PagedList;

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
            Dictionary<Track, List<Artist>> tracks = GetFeatureTrack();
            Dictionary<Track, List<Artist>> popularTracks = GetMostPopularTrack();
            Dictionary<Album, List<Artist>> albums = GetFeatureAlbum();
            List<Artist> artists = GetFeatureArtists();
            ViewData["tracks"] = tracks;
            ViewData["popularTracks"] = popularTracks;
            ViewData["artists"] = artists;
            ViewData["albums"] = albums;
            return View("Index");
        }

        public IActionResult Search(string key, string sortby, string[] genres, string[] artists, int page)
        {
            string sort = string.IsNullOrEmpty(sortby) ? "newest" : sortby;
            page = page < 1 ? 1 : page;
            key = string.IsNullOrEmpty(key) ? "" : key;
            var trackList = GetListTrackByFilter(artists, genres, key, sort);
            page = (page > (trackList.Count / 12)) ? trackList.Count / 12 : page;
            ViewData["artists"] = GetAllArtistAsMultiSelectList();
            ViewData["genres"] = GetAllGenresAsMultiSelectList();
            ViewData["sort"] = sort;
            ViewData["page"] = page;
            ViewData["key"] = key;
            ViewData["artistsId"] = artists;
            ViewData["genresId"] = genres;
            return View(trackList.ToPagedList<Track>(pageNumber: page, pageSize: 12));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
