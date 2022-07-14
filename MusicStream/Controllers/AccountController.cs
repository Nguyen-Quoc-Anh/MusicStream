using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStream.Controllers.Logic;
using MusicStream.Logic;
using MusicStream.Models;
using MusicStream.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccountController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }

        [Route("google-signup")]
        public IActionResult GoogleSignup()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.ActionLink("google-response") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse()
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
                ViewBag.Message = "Đăng ký thất bại.";
                return View("Message");
            }
            else
            {
                Account account = new Account();
                account.AccountId = id;
                account.Email = email;
                account.RoleId = 2;
                account.Password = "";

                account.Fullname = name;
                account.Image = "/img/avatar/avatar.jpg";

                RegisterAccount(account);
                Account acc = AccountLogic.SignInWithGoogle(email, id);
                CookieOptions cookie = new CookieOptions();
                cookie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Append("account", acc.AccountId, cookie);
                HttpContext.Session.SetString("account", JsonConvert.SerializeObject(acc));
                ViewBag.Message = "Đăng ký thành công";
                return View("Message");
            }
        }

        [Route("google-signin")]
        public IActionResult GoogleSignin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.ActionLink("google-signin-response") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        [Route("google-signin-response")]
        public async Task<IActionResult> GoogleSigninResponse()
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
            if (!IsIdExist(id) && !IsEmailAlreadyInUse(email))
            {
                Account account = new Account();
                account.AccountId = id;
                account.Email = email;
                account.RoleId = 2;
                account.Password = "";

                account.Fullname = name;
                account.Image = "/img/avatar/avatar.jpg";

                RegisterAccount(account);
                Account acc = AccountLogic.SignInWithGoogle(email, id);
                CookieOptions cookie = new CookieOptions();
                cookie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Append("account", acc.AccountId, cookie);
                HttpContext.Session.SetString("account", JsonConvert.SerializeObject(acc));
                ViewBag.Message = "Đăng nhập thành công";
                return View("Message");
            }
            else
            {
                Account account = AccountLogic.SignInWithGoogle(email, id);
                CookieOptions cookie = new CookieOptions();
                cookie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Append("account", account.AccountId, cookie);
                HttpContext.Session.SetString("account", JsonConvert.SerializeObject(account));
                ViewBag.Message = "Đăng nhập thành công";
                return View("Message");
            }
        }

        [Route("favourite-song")]
        public IActionResult FavouriteSong(int page)
        {
            int pageNumber = page;
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return Redirect("/error");
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

        [Route("forgot")]
        public async Task<IActionResult> ForgotPasswordAsync(string Email, string VerifyCode, string Password, string RePassword)
        {
            string step = HttpContext.Session.GetString("ForgotPasswordStep");

            if (step == null || step.Equals("step1"))
            {
                if (Email != null)
                {
                    if (AccountLogic.IsEmailAlreadyInUse(Email))
                    {
                        string verifyCode = Util.RandomString(20);
                        MailRequest mailRequest = new MailRequest();
                        mailRequest.Subject = "Mã xác minh của bạn";
                        mailRequest.Body = "Mã xác minh: " + verifyCode;
                        mailRequest.ToEmail = Email;
                        var sendmailservice = HttpContext.RequestServices.GetService<ISendMailService>();
                        bool success = await sendmailservice.SendEmail(mailRequest);
                        if (success)
                        {
                            HttpContext.Session.SetString("VerifyCode", verifyCode);
                            HttpContext.Session.SetString("ForgotPasswordStep", "step2");
                            ViewBag.Email = Email;
                            HttpContext.Session.SetString("AccountId", GetAccountIdByEmail(Email));
                        }
                        else
                        {
                            ViewBag.Message = "Gửi email thất bại vui lòng thử lại sau.";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Email không hợp lệ.";
                    }
                }
            }
            else if (step.Equals("step2"))
            {
                string verifyCode = HttpContext.Session.GetString("VerifyCode");
                if (verifyCode.Equals(VerifyCode))
                {
                    HttpContext.Session.SetString("ForgotPasswordStep", "step3");
                }
                else
                {
                    ViewBag.Message = "Mã xác minh chưa chính xác vui lòng thử lại";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(RePassword))
                {
                    ViewBag.Message = "Vui lòng nhập đầy đủ tất cả các trường.";
                }
                if (!Password.Equals(RePassword))
                {
                    ViewBag.Message = "Mật khẩu nhập lại và mật khẩu không giống nhau.";
                }
                else
                {
                    bool success = AccountLogic.ChangePassword(HttpContext.Session.GetString("AccountId"), Util.EncodePassword(Password));
                    if (success)
                    {
                        ViewBag.Message = "Đổi mật khẩu thành công. Đến trang đăng nhập sau 3 giây";
                        ViewBag.Success = true;
                    }
                    else
                    {
                        ViewBag.Message = "Đổi mật khẩu thất bại.";
                    }
                }
            }
            return View("ForgotPassword");
        }

        [Route("profile")]
        public IActionResult Profile()
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return Redirect("/error");
            }
            if (string.IsNullOrEmpty(account.Password))
            {
                ViewBag.ThirdParty = true;
            }
            return View("Profile", account);
        }

        [Route("changeusername"), HttpPut]
        public string ChangeUsername(string name)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("changeusername", false));
            }
            bool success = AccountLogic.ChangeUsername(account, name);
            if (success)
            {
                account.Fullname = name;
                HttpContext.Session.SetString("account", JsonConvert.SerializeObject(account));
            }
            return JsonConvert.SerializeObject(new Models.Action("changeusername", success));
        }

        [Route("changepassword"), HttpPut]
        public string ChangePassword(string oldpass, string newpass)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("changepass", false));
            }

            Account tempAcc = AccountLogic.SignIn(account.Email, oldpass);
            if (tempAcc != null)
            {
                bool success = AccountLogic.ChangePassword(account.AccountId, newpass);
                return JsonConvert.SerializeObject(new Models.Action("changepass", success));
            }
            else
            {
                return JsonConvert.SerializeObject(new Models.Action("changepass", false, "Mật khẩu không chính xác."));
            }
        }

        [Route("setpassword"), HttpPut]
        public string SetPassword(string newpass)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return JsonConvert.SerializeObject(new Models.Action("setpass", false));
            }

            if (string.IsNullOrEmpty(account.Password))
            {
                bool success = AccountLogic.ChangePassword(account.AccountId, Util.EncodePassword(newpass));
                return JsonConvert.SerializeObject(new Models.Action("setpass", success));
            }
            else
            {
                return JsonConvert.SerializeObject(new Models.Action("setpass", false));
            }
        }

        [HttpPost, Route("changeavatar")]
        public async Task<ActionResult> ChangeAvatar(IFormFile file)
        {
            Account account = Util.CheckLogged(HttpContext, Request);
            if (account == null)
            {
                return Redirect("/error");
            }
            string extension = file.ContentType.ToLower().Split("/")[1];
            extension = extension.Equals("jpeg") ? "jpg" : extension;
            if (file != null)
            {
                if (account.Image.Contains("avatar.jpg"))
                {
                    await Util.UploadedFile(file, webHostEnvironment, $"{account.AccountId}.{extension}", "img/avatar/");
                    HttpContext.Session.SetString("edit", "success");
                    AccountLogic.ChangeUserAvatar(account, $"/img/avatar/{account.AccountId}.{extension}");
                }
                else
                {
                    bool deleteSuccess = Util.DeleteFile(webHostEnvironment, account.Image.Split("/")[3], "img/avatar/");
                    if (deleteSuccess)
                    {
                        await Util.UploadedFile(file, webHostEnvironment, $"{account.AccountId}.{extension}", "img/avatar/");
                        HttpContext.Session.SetString("edit", "success");
                    }
                    else
                    {
                        HttpContext.Session.SetString("edit", "failed");
                    }
                }
            }
            account = Util.CheckLogged(HttpContext, Request);
            HttpContext.Session.SetString("account", JsonConvert.SerializeObject(account));
            return Redirect("/account/profile");
        }
    }
}
