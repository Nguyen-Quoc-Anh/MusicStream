using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicStream.Controllers.Logic;
using MusicStream.Logic;
using MusicStream.Models;
using Newtonsoft.Json;

namespace MusicStream.Controllers
{
    public class CommentController : Controller
    {
        [HttpPost]
        public IActionResult AddComment(string text, string Id)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return Redirect("/error");
            }
            Album album = AlbumLogic.GetAlbumDetails(Id, account.AccountId);
            if (album == null)
            {
                return Redirect("/error");
            }
            string commentId = Util.RandomString(20);
            while (CommentLogic.CheckCommentExistById(commentId))
            {
                commentId = Util.RandomString(20);
            }
            bool success = CommentLogic.AddComment(account.AccountId, text, Id, commentId);
            if (success)
            {
                HttpContext.Session.SetString("addcomment", "success");
            }
            else
            {
                HttpContext.Session.SetString("addcomment", "failed");
            }
            return Redirect($"/album/detail/{Id}");
        }

        [HttpPost]
        public IActionResult EditComment(string text, string Id, string commentId)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return Redirect("/error");
            }
            Album album = AlbumLogic.GetAlbumDetails(Id, account.AccountId);
            if (album == null)
            {
                return Redirect("/error");
            }
            if (CommentLogic.CheckCommentExistById(commentId))
            {
                bool success = CommentLogic.EditComment(text, commentId);
                if (success)
                {
                    HttpContext.Session.SetString("editcomment", "success");
                }
                else
                {
                    HttpContext.Session.SetString("editcomment", "failed");
                }
            }
            else
            {
                HttpContext.Session.SetString("editcomment", "failed");
            }
            return Redirect($"/album/detail/{Id}");
        }

        [HttpDelete]
        public string DeleteComment(string Id)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("deletecomment", false));
            }

            if (CommentLogic.CheckCommentExistById(Id))
            {
                bool success = CommentLogic.DeleteCommentById(Id);
                return JsonConvert.SerializeObject(new Models.Action("deletecomment", success));
            }
            else
            {
                return JsonConvert.SerializeObject(new Models.Action("deletecomment", false));
            }
        }
    }
}
