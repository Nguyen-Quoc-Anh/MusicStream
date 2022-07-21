using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicStream.Controllers.Logic;
using MusicStream.Logic;
using MusicStream.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace MusicStream.Controllers
{
    public class PlayListController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        public PlayListController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }
        public IActionResult List(int page)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return Redirect("/error");
            }
            if (page < 1)
            {
                page = 1;
            }
            List<Playlist> playlist = PlayListLogic.GetAllPlayListAndFollowPlayListByAccountId(account.AccountId);
            if (account != null)
            {
                ViewData["accountId"] = account.AccountId;
            }
            return View("List", playlist.ToPagedList<Playlist>(pageNumber: page, pageSize: 12));
        }

        public IActionResult Detail(string id)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            Playlist playlist = account == null ? PlayListLogic.GetPlaylistById(id) : PlayListLogic.GetPlaylistById(id, account.AccountId);
            if (playlist == null)
            {
                return Redirect("/error");
            }
            if (Convert.ToBoolean(playlist.IsPrivate))
            {
                if (account.AccountId != playlist.AccountId)
                {
                    return Redirect("/error");
                }
            }
            if (account != null)
            {
                if (account.AccountId == playlist.AccountId)
                {
                    ViewData["isOwner"] = true;
                    ViewBag.AccountId = account.AccountId;
                }
                ViewData["playLists"] = PlayListLogic.GetAllPlayListByAccountId(account.AccountId);
                if (PlayListLogic.IsAccountFollowPlaylist(account.AccountId, id))
                {
                    ViewBag.IsAccountFollow = true;
                }
            }

            ViewBag.ReturnUrl = $"/playlist/detail/{id}";
            return View("Detail", playlist);
        }

        public string AddToWishList(string id)
        {
            List<Track> tracks = TrackLogic.GetTrackListByPlayList(id);
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            return JsonConvert.SerializeObject(tracks, settings);
        }

        [HttpPost]
        public string AddToPlaylist(string playlistId, string trackId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("addtoplaylist", false));
            }
            if (PlayListLogic.CheckTrackIsExistInPlaylist(playlistId, trackId))
            {
                return JsonConvert.SerializeObject(new Models.Action("addtoplaylist", false));
            }
            else
            {
                if (PlayListLogic.AddToPlayList(playlistId, trackId))
                {
                    return JsonConvert.SerializeObject(new Models.Action("addtoplaylist", true));
                }
                return JsonConvert.SerializeObject(new Models.Action("addtoplaylist", false));
            }
        }

        [HttpPost]
        public string RemoveFromPlaylist(string playlistId, string trackId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("removefromplaylist", false));
            }
            if (!PlayListLogic.CheckTrackIsExistInPlaylist(playlistId, trackId))
            {
                return JsonConvert.SerializeObject(new Models.Action("removefromplaylist", false));
            }
            else
            {
                if (PlayListLogic.RemoveFromPlayList(playlistId, trackId))
                {
                    return JsonConvert.SerializeObject(new Models.Action("removefromplaylist", true));
                }
                return JsonConvert.SerializeObject(new Models.Action("removefromplaylist", false));
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddNewPlaylist(IFormFile file, string name, string isPrivate)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return Redirect("/error");
            }
            string playListId = Util.RandomString(20);
            while (true)
            {
                if (PlayListLogic.CheckPlayListIdIsExist(playListId))
                {
                    playListId = Util.RandomString(20);
                }
                else
                {
                    break;
                }
            }
            string imageUrl;
            if (file != null)
            {
                string extension = Path.GetExtension(file.FileName);

                imageUrl = await Util.UploadedFile(file, webHostEnvironment, playListId + extension, "img/playlist/");
            }
            else
            {
                imageUrl = "/img/playlist/index.jpg";
            }
            Playlist playlist = new Playlist();
            playlist.PlaylistId = playListId;
            playlist.Name = name;
            playlist.AccountId = account.AccountId;
            playlist.Image = imageUrl;
            playlist.IsPrivate = isPrivate != null;
            bool success = PlayListLogic.AddNewPlayList(playlist);
            if (success)
            {
                HttpContext.Session.SetString("addPlaylistsuccess", "true");
            }
            else
            {
                HttpContext.Session.SetString("addPlaylistsuccess", "false");
            }
            return Redirect("/playlist/list");
        }


        [HttpDelete]
        public async Task<string> DeletePlaylist(string Id)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("deleteplaylist", false));
            }
            if (!PlayListLogic.CheckPlayListIdIsExist(Id))
            {
                return JsonConvert.SerializeObject(new Models.Action("deleteplaylist", false));
            }
            Playlist playlist = PlayListLogic.GetPlaylistById(Id);
            if (playlist.Image.Contains("index.jpg"))
            {
                if (account.AccountId != playlist.AccountId)
                {
                    return JsonConvert.SerializeObject(new Models.Action("deleteplaylist", false));
                }

                bool success = await PlayListLogic.DeletePlayListById(Id);
                return JsonConvert.SerializeObject(new Models.Action("deleteplaylist", success));
            }
            else
            {
                bool deleteSuccess = Util.DeleteFile(webHostEnvironment, playlist.Image.Split("/")[3], "img/playlist/");
                if (account.AccountId != playlist.AccountId || !deleteSuccess)
                {
                    return JsonConvert.SerializeObject(new Models.Action("deleteplaylist", false));
                }

                bool success = await PlayListLogic.DeletePlayListById(Id);
                return JsonConvert.SerializeObject(new Models.Action("deleteplaylist", success));
            }

        }

        public IActionResult Search(string name, int page, string sort)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            sort = string.IsNullOrEmpty(sort) ? "popular" : sort;
            name = string.IsNullOrEmpty(name) ? "" : name;
            page = page == 0 ? 1 : page;
            ViewBag.Sort = sort;
            ViewBag.Name = name;
            if (account != null)
            {
                ViewBag.AccountId = account.AccountId;
            }
            List<Playlist> playlists = PlayListLogic.GetPlaylistsBySearch(name, sort);
            return View("Search", playlists.ToPagedList<Playlist>(pageNumber: page, pageSize: 12));
        }

        [HttpPost]
        public string FollowPlaylist(string Id)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("follow", false));
            }
            if (PlayListLogic.IsAccountFollowPlaylist(account.AccountId, Id))
            {
                return JsonConvert.SerializeObject(new Models.Action("follow", false));
            }
            else
            {
                bool success = PlayListLogic.FollowPlaylist(account.AccountId, Id);
                return JsonConvert.SerializeObject(new Models.Action("follow", success));
            }
        }

        [HttpPost]
        public string UnFollowPlaylist(string Id)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("follow", false));
            }
            if (PlayListLogic.IsAccountFollowPlaylist(account.AccountId, Id))
            {
                bool success = PlayListLogic.UnFollowPlaylist(account.AccountId, Id);
                return JsonConvert.SerializeObject(new Models.Action("follow", success));
            }
            else
            {
                return JsonConvert.SerializeObject(new Models.Action("follow", false));
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditPlaylist(IFormFile file, string name, string isPrivate, string playlistId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return Redirect("/error");
            }

            if (PlayListLogic.CheckPlayListIdIsExist(playlistId))
            {
                Playlist playlist = PlayListLogic.GetPlaylistById(playlistId);
                if (!string.IsNullOrEmpty(name))
                {
                    playlist.Name = name;
                }
                playlist.IsPrivate = isPrivate != null;
                if (file != null)
                {
                    string extension = Path.GetExtension(file.FileName);
                    if (playlist.Image.Contains("index.jpg"))
                    {
                        await Util.UploadedFile(file, webHostEnvironment, $"{playlistId}{extension}", "img/playlist/");
                        playlist.Image = $"/img/playlist/{playlistId}{extension}";
                    }
                    else
                    {
                        bool deleteSuccess = Util.DeleteFile(webHostEnvironment, playlist.Image.Split("/")[3], "img/playlist/");
                        if (deleteSuccess)
                        {
                            await Util.UploadedFile(file, webHostEnvironment, $"{playlistId}{extension}", "img/playlist/");
                            playlist.Image = $"/img/playlist/{playlistId}{extension}";
                        }
                        else
                        {
                            HttpContext.Session.SetString("edit", "failed");
                            return Redirect($"/playlist/detail/{playlistId}");
                        }
                    }

                }
                bool updateSuccess = PlayListLogic.EditPlaylist(playlist);
                if (updateSuccess)
                {
                    HttpContext.Session.SetString("edit", "success");
                }
                else
                {
                    HttpContext.Session.SetString("edit", "failed");
                }
            }
            else
            {
                HttpContext.Session.SetString("edit", "failed");
            }

            return Redirect($"/playlist/detail/{playlistId}");
        }
    }
}
