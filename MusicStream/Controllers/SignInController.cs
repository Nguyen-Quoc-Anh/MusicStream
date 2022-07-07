using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicStream.Controllers.Logic;
using MusicStream.Models;
using Newtonsoft.Json;
using System;

namespace MusicStream.Controllers
{
    [Route("signin")]
    public class SignInController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {

            return View("Index");
        }

        [HttpPost]
        public IActionResult SignIn(string Email, string Password, string Remember)
        {
            Account account = AccountLogic.SignIn(Email, Password);
            if (account != null)
            {
                if (Remember != null)
                {
                    CookieOptions cookie = new CookieOptions();
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Append("account", account.AccountId, cookie);
                }
                HttpContext.Session.SetString("account", JsonConvert.SerializeObject(account));
                return Redirect("/");
            }
            else
            {
                ViewData["Message"] = "Sai tài khoản hoặc mật khẩu.";
                return View("Index");
            }
        }
    }
}
