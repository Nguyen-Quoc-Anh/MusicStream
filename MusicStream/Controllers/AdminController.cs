using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MusicStream.Models;
using MusicStream.Controllers.Logic;
using X.PagedList;
using System.Linq;
using Newtonsoft.Json;
using MusicStream.Logic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace MusicStream.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        public AdminController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }
        [Route("genre")]
        public IActionResult Genre(int page, string search)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            page = page == 0 ? 1 : page;
            List<Genre> genreList = GenreLogic.GetAllGenres();
            if (!string.IsNullOrEmpty(search))
            {
                genreList = genreList.Where(g => g.Name.ToLower().Contains(search.ToLower())).ToList();
            }
            ViewBag.Search = search;
            page = (page > ((genreList.Count % 20 == 0) ? genreList.Count % 20 : genreList.Count % 20 + 1)) ? 1 : page;
            return View("Genre", genreList.ToPagedList<Genre>(pageNumber: page, pageSize: 20));
        }

        [HttpDelete, Route("deletegenre")]
        public string DeleteGenre(string genreId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return JsonConvert.SerializeObject(new Models.Action("deletegenre", false));
            }
            if (GenreLogic.CheckGenreExistById(genreId))
            {
                bool success = GenreLogic.DeleteGenreById(genreId);
                return JsonConvert.SerializeObject(new Models.Action("deletegenre", success));
            }
            else
            {
                return JsonConvert.SerializeObject(new Models.Action("deletegenre", false));
            }
        }

        [HttpPost, Route("addnewgenre")]
        public JsonResult AddNewGenre(string genreName)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Json(JsonConvert.SerializeObject(new Models.Action("addgenres", false)));
            }
            string genreId = Util.RandomString(8);
            while (GenreLogic.CheckGenreExistById(genreId))
            {
                genreId = Util.RandomString(8);
            }
            bool success = GenreLogic.AddNewGenre(new Genre() { GenreId = genreId, Name = genreName });
            return Json(JsonConvert.SerializeObject(new Models.Action("addgenres", success)));
        }

        [HttpPut, Route("editgenre")]
        public JsonResult EditGenre(string genreId, string genreName)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Json(JsonConvert.SerializeObject(new Models.Action("addgenres", false)));
            }
            if (GenreLogic.CheckGenreExistById(genreId))
            {
                bool success = GenreLogic.UpdateGenre(new Genre() { GenreId = genreId, Name = genreName });
                return Json(JsonConvert.SerializeObject(new Models.Action("addgenres", success)));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new Models.Action("addgenres", false)));
            }
        }

        [Route("artist")]
        public IActionResult ArtistList(int page, string search)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            List<Artist> artistList = ArtistLogic.GetAllArtist();
            page = page == 0 ? 1 : page;
            if (!string.IsNullOrEmpty(search))
            {
                artistList = artistList.Where(a => a.Fullname.ToLower().Contains(search.ToLower())).ToList();
            }
            page = (page > ((artistList.Count % 20 == 0) ? artistList.Count % 20 : artistList.Count % 20 + 1)) ? 1 : page;
            ViewBag.Search = search;
            return View("Artist", artistList.ToPagedList<Artist>(pageNumber: page, pageSize: 20));
        }

        [Route("addartist"), HttpPost]
        public async Task<IActionResult> AddArtistAsync(string name, IFormFile file, string description)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            string artistId = Util.RandomString(8);
            while (ArtistLogic.CheckArtistIdExtist(artistId))
            {
                artistId = Util.RandomString(8);
            }
            string imageUrl;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                imageUrl = await Util.UploadedFile(file, webHostEnvironment, artistId + extension, "img/artist/");
            }
            else
            {
                HttpContext.Session.SetString("addartist", "failed");
                return Redirect("/admin/artist");
            }
            Artist artist = new Artist() { ArtistId = artistId, Fullname = name, Description = description, Image = imageUrl };
            bool success = ArtistLogic.InsertArtist(artist);
            HttpContext.Session.SetString("addartist", success ? "success" : "failed");
            return Redirect("/admin/artist");
        }

        [Route("editartist"), HttpPost]
        public async Task<IActionResult> EditArtistAsync(string artistId, string name, IFormFile file, string description)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            Artist artist = ArtistLogic.GetArtistByID(artistId);
            if (artist == null)
            {
                HttpContext.Session.SetString("editartist", "failed");
                return Redirect("/admin/artist");
            }
            string imageUrl;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                bool deleteSuccess = Util.DeleteFile(webHostEnvironment, artist.Image.Split("/")[3], "img/artist/");
                if (deleteSuccess)
                {
                    imageUrl = await Util.UploadedFile(file, webHostEnvironment, artistId + extension, "img/artist/");
                }
                else
                {
                    HttpContext.Session.SetString("editartist", "failed");
                    return Redirect("/admin/artist");
                }
            }
            else
            {
                imageUrl = artist.Image;
            }
            artist.Fullname = name;
            artist.Description = description;
            artist.Image = imageUrl;
            bool success = ArtistLogic.UpdateArtist(artist);
            HttpContext.Session.SetString("editartist", success ? "success" : "failed");
            return Redirect("/admin/artist");
        }

        [Route("deleteartist"), HttpDelete]
        public JsonResult DeleteArtist(string artistId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Json(JsonConvert.SerializeObject(new Models.Action("deleteartist", false)));
            }
            if (ArtistLogic.CheckArtistIdExtist(artistId))
            {
                bool success = ArtistLogic.DeleteArtist(artistId);
                return Json(JsonConvert.SerializeObject(new Models.Action("deleteartist", success)));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new Models.Action("deleteartist", false)));
            }

        }

        [Route("getartist"), HttpGet]
        public JsonResult GetArtistInfo(string artistId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Json("null");
            }
            Artist artist = ArtistLogic.GetArtistByID(artistId);
            return Json(JsonConvert.SerializeObject(artist));
        }

        [Route("album")]
        public IActionResult AlbumList(int page, string search, string albumId, string[] artistId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            if (string.IsNullOrEmpty(albumId))
            {
                List<Album> albumList = AlbumLogic.GetAllAlbum();
                page = page == 0 ? 1 : page;
                if (!string.IsNullOrEmpty(search))
                {
                    albumList = albumList.Where(a => a.AlbumName.ToLower().Contains(search.ToLower())).ToList();
                }
                page = (page > ((albumList.Count % 20 == 0) ? albumList.Count % 20 : albumList.Count % 20 + 1)) ? 1 : page;
                ViewBag.Search = search;
                return View("Album", albumList.ToPagedList<Album>(pageNumber: page, pageSize: 20));
            }
            else
            {
                Album album = AlbumLogic.GetAlbumDetails(albumId);
                if (album == null)
                {
                    return Redirect("/error");
                }
                return View("AlbumDetail", album);
            }
        }

        [Route("addtrack"), HttpGet]
        public IActionResult AddTrack(string albumId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            var genres = GenreLogic.GetAllGenres().Select(g => new SelectListItem { Text = g.Name, Value = g.GenreId }).ToList();
            var artists = ArtistLogic.GetAllArtist().Select(a => new SelectListItem { Text = a.Fullname, Value = a.ArtistId }).ToList();
            ViewBag.AlbumId = albumId;
            ViewBag.Genres = genres;
            ViewBag.Artists = artists;
            return View("AddTrack");
        }

        [Route("addtrack"), HttpPost]
        public async Task<IActionResult> PostTrack(string albumId, string name, string[] artists, string[] genres, IFormFile file, IFormFile mp3, int duration)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            string trackId = Util.RandomString(8);
            while (TrackLogic.CheckTrackExistById(trackId))
            {
                trackId = Util.RandomString(8);
            }
            string imageUrl;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                imageUrl = await Util.UploadedFile(file, webHostEnvironment, trackId + extension, "img/track/");
            }
            else
            {
                imageUrl = AlbumLogic.GetAlbumDetails(albumId).Image;
            }
            string mp3Url;
            if (mp3 != null)
            {
                string mp3Extension = Path.GetExtension(mp3.FileName);
                mp3Url = await Util.UploadedFile(mp3, webHostEnvironment, Path.GetFileNameWithoutExtension(mp3.FileName) + trackId + mp3Extension, "mp3/");
            }
            else
            {
                HttpContext.Session.SetString("addtrack", "failed");
                return Redirect($"/admin/album?albumId={albumId}");
            }

            Track track = new Track() { TrackId = trackId, AlbumId = albumId, Name = name, Image = imageUrl, Mp3 = mp3Url, Duration = duration };
            bool success = TrackLogic.InsertTrack(track, artists, genres);
            HttpContext.Session.SetString("addtrack", success ? "success" : "failed");
            return Redirect($"/admin/album?albumId={albumId}");
        }

        [Route("deletetrack"), HttpDelete]
        public JsonResult DeleteTrack(string trackId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Json(JsonConvert.SerializeObject(new Models.Action("deletetrack", false)));
            }
            if (TrackLogic.CheckTrackExistById(trackId))
            {
                bool success = TrackLogic.DeleteTrack(trackId);
                return Json(JsonConvert.SerializeObject(new Models.Action("deletetrack", success)));
            }
            else
            {
                return Json(JsonConvert.SerializeObject(new Models.Action("deletetrack", false)));
            }
        }

        [Route("edittrack"), HttpGet]
        public IActionResult EditTrack(string trackId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            Track track = TrackLogic.GetTrackByID(trackId);
            if (track == null) return Redirect("/error");
            var genres = GenreLogic.GetAllGenres().Select(g => new SelectListItem { Text = g.Name, Value = g.GenreId }).ToList();
            var artists = ArtistLogic.GetAllArtist().Select(a => new SelectListItem { Text = a.Fullname, Value = a.ArtistId }).ToList();
            ViewBag.Genres = genres;
            ViewBag.Artists = artists;
            return View("EditTrack", track);
        }

        [Route("edittrack"), HttpPost]
        public async Task<IActionResult> DoEditTrack(string trackId, string name, string[] artists, string[] genres, IFormFile file, IFormFile mp3, int duration)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            Track track = TrackLogic.GetTrackByID(trackId);
            if (track == null) return Redirect("/error");
            track.Name = name;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (track.Image.Contains("img/track/"))
                {
                    Util.DeleteFile(webHostEnvironment, track.Image.Split("/")[track.Image.Split("/").Length - 1], "img/track/");
                }
                track.Image = await Util.UploadedFile(file, webHostEnvironment, trackId + extension, "img/track/");
            }
            if (mp3 != null)
            {
                string mp3Extension = Path.GetExtension(mp3.FileName);
                if (track.Mp3.Contains("mp3/"))
                {
                    Util.DeleteFile(webHostEnvironment, track.Mp3.Split("/")[track.Mp3.Split("/").Length - 1], "mp3/");
                }
                track.Mp3 = await Util.UploadedFile(mp3, webHostEnvironment, Path.GetFileNameWithoutExtension(mp3.FileName) + trackId + mp3Extension, "mp3/");
                track.Duration = duration;
            }
            bool success = TrackLogic.UpdateTrack(track, genres, artists);
            HttpContext.Session.SetString("edittrack", success ? "success" : "failed");
            return Redirect("/admin/album?albumId=" + track.AlbumId);
        }

        [Route("addalbum"), HttpPost]
        public async Task<IActionResult> AddAlbum(string name, IFormFile file, DateTime releaseDate)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            string albumId = Util.RandomString(8);
            while (AlbumLogic.GetAlbumDetails(albumId) != null)
            {
                albumId = Util.RandomString(8);
            }
            string imageUrl;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                {
                    HttpContext.Session.SetString("addAlbum", "failed");
                    return Redirect("/admin/album");
                }
                imageUrl = await Util.UploadedFile(file, webHostEnvironment, albumId + extension, "img/album/");
            }
            else
            {
                HttpContext.Session.SetString("addAlbum", "failed");
                return Redirect("/admin/album");
            }
            Album album = new Album() { AlbumId = albumId, AlbumName = name, Image = imageUrl, ReleaseDate = releaseDate };
            bool success = AlbumLogic.InsertAlbum(album);
            HttpContext.Session.SetString("addAlbum", success ? "success" : "failed");
            return Redirect("/admin/album");
        }

        [Route("deletealbum"), HttpDelete]
        public Task<JsonResult> DeleteAlbum(string albumId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Task.FromResult(Json(JsonConvert.SerializeObject(new Models.Action("deletealbum", false))));
            }
            Album album = AlbumLogic.GetAlbumDetails(albumId);
            if (album == null)
            {
                return Task.FromResult(Json(JsonConvert.SerializeObject(new Models.Action("deletealbum", false))));
            }
            else
            {
                bool success = AlbumLogic.DeleteAlbum(albumId);
                return Task.FromResult(Json(JsonConvert.SerializeObject(new Models.Action("deletealbum", success))));
            }
        }

        [Route("editalbum"), HttpPost]
        public async Task<IActionResult> EditAlbum(string albumId, string name, IFormFile file, DateTime releaseDate)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null || account.RoleId != 1)
            {
                return Redirect("/error");
            }
            Album album = AlbumLogic.GetAlbumDetails(albumId);
            if (album == null)
            {
                HttpContext.Session.SetString("editAlbum", "failed");
                return Redirect("/admin/album");
            }
            string imageUrl;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension != ".jpg" && extension != ".png" && extension != ".jpeg")
                {
                    HttpContext.Session.SetString("editAlbum", "failed");
                    return Redirect("/admin/album");
                }
                if (album.Image.Contains("img/album/"))
                {
                    Util.DeleteFile(webHostEnvironment, album.Image.Split("/")[album.Image.Split("/").Length - 1], "img/album/");
                }
                imageUrl = await Util.UploadedFile(file, webHostEnvironment, albumId + extension, "img/album/");
                album.Image = imageUrl;
            }
            album.AlbumName = name;
            album.ReleaseDate = releaseDate;
            bool success = AlbumLogic.UpdateAlbum(album);
            HttpContext.Session.SetString("editAlbum", success ? "success" : "failed");
            return Redirect("/admin/album");
        }
    }
}
