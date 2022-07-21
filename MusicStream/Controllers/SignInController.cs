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
        public IActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Index");
        }

        [HttpPost]
        public IActionResult SignIn(string Email, string Password, string Remember, string returnUrl)
        {
            Account account = AccountLogic.SignIn(Email, Password);
            if (account != null)
            {
                if (!Convert.ToBoolean(account.Status))
                {
                    ViewData["Message"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với quản trị viên để biết thêm chi tiết.";
                }
                if (Remember != null)
                {
                    CookieOptions cookie = new CookieOptions();
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Append("account", account.AccountId, cookie);
                }
                HttpContext.Session.SetString("account", JsonConvert.SerializeObject(account));
                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("/");
                }
            }
            else
            {
                ViewData["Message"] = "Sai tài khoản hoặc mật khẩu.";
                return View("Index");
            }
        }
    }
}
