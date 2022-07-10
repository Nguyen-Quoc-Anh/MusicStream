using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicStream.Logic;
using MusicStream.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using static MusicStream.Controllers.Logic.AccountLogic;
using static MusicStream.Controllers.Logic.TrackLogic;
namespace MusicStream.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {


        [Route("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<JsonResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value,
            });
            claims = claims.ToList();
            string id = claims.ElementAt(0).Value;
            string name = claims.ElementAt(1).Value;
            string email = claims.ElementAt(4).Value;
            if (IsEmailAlreadyInUse(email) || IsIdExist(id))
            {
                Action action = new Action("Sign In", false);
                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(action));
            }
            else
            {
                Account account = new Account();
                account.AccountId = id;
                account.Email = email;
                account.RoleId = 2;
                account.Password = "";

                account.Fullname = name;
                account.Image = "~/img/avatar/avatar.jpg";

                RegisterAccount(account);

                Action action = new Action("Sign In", true);
                return Json(Newtonsoft.Json.JsonConvert.SerializeObject(action));
            }
        }

        [Route("favourite-song")]
        public IActionResult FavouriteSong(int page)
        {
            int pageNumber = page;
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return NotFound();
            }
            List<Track> tracksList = GetFavouriteListTrack(account.AccountId);

            page = page < 1 ? 1 : page;
            return View("FavouriteSong", tracksList.ToPagedList<Track>(pageNumber: page, pageSize: 12));
        }

        [Route("logout")]
        public IActionResult LogOut()
        {
            Response.Cookies.Delete("account");
            HttpContext.Session.Remove("account");
            return LocalRedirect("/");
        }
    }
}
