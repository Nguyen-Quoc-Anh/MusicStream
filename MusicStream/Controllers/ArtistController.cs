using Microsoft.AspNetCore.Mvc;
using MusicStream.Models;
using System.Collections.Generic;
using static MusicStream.Controllers.Logic.ArtistLogic;
using static MusicStream.Controllers.Logic.AlbumLogic;
using X.PagedList;

namespace MusicStream.Controllers
{
    public class ArtistController : Controller
    {
        public IActionResult Detail(string id)
        {
            Artist artist = GetArtistByID(id);
            if (artist == null)
                return NotFound();
            List<Artist> artists = GetFeatureArtists();
            List<Album> albums = GetAlbumByArtistId(id);
            ViewData["albums"] = albums;
            ViewData["artists"] = artists;
            return View(artist);
        }

        public IActionResult Search(string name, int page, string sort)
        {
            sort = string.IsNullOrEmpty(sort) ? "asc" : sort;
            name = string.IsNullOrEmpty(name) ? "" : name;
            page = page == 0 ? 1 : page;
            List<Artist> artists = GetArtistsByName(name, sort);
            ViewBag.Sort = sort;
            ViewBag.Name = name;
            return View("Search", artists.ToPagedList<Artist>(pageNumber: page, pageSize: 12));
        }
    }
}
