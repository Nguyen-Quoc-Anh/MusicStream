using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStream.Logic;
using MusicStream.Models;
using MusicStream.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static MusicStream.Controllers.Logic.AccountLogic;

namespace MusicStream.Controllers
{
    [Route("signup")]
    public class SignUpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("new-account"), HttpPost]
        public async Task<IActionResult> NewAccount(string Name, string Email, string Password)
        {
            string accountID = Util.RandomString(20);
            string message = null;
            while (IsIdExist(accountID))
            {
                accountID = Util.RandomString(20);
            }
            if (IsEmailAlreadyInUse(Email))
            {
                message = "Email này đã được sử dụng. Vui lòng đăng ký với một email khác.";
                ViewData["Message"] = message;
                return View("Index");
            }
            Account account = new();
            account.Email = Email;
            account.Password = Util.EncodePassword(Password);
            account.AccountId = accountID;
            account.RoleId = 2;
            account.Fullname = Name;
            account.Image = "~/img/avatar/avatar.jpg";
            string verifiCode = Util.RandomString(10);
            MailRequest mailRequest = new MailRequest();
            mailRequest.Subject = "Mã xác minh của bạn";
            mailRequest.Body = "Mã xác minh: " + verifiCode;
            mailRequest.ToEmail = Email;
            var sendmailservice = HttpContext.RequestServices.GetService<ISendMailService>();
            bool success = await sendmailservice.SendEmail(mailRequest);
            if (success)
            {
                HttpContext.Session.SetString("TempAccount", JsonConvert.SerializeObject(account));
                HttpContext.Session.SetString("SignUp", "step2");
                HttpContext.Session.SetString("VerificationCode", verifiCode);
                return View("VerifyAccount");
            }
            else
            {
                message = "Không thể gửi email. Thử lại vào lần sau";
                ViewData["Message"] = message;
                return View("Index");
            }
        }

        [Route("verify-account"), HttpPost]
        public IActionResult AddAccount(string VerifyCode)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SignUp")) || !HttpContext.Session.GetString("SignUp").Equals("step2"))
            {
                return NotFound();
            }
            if (HttpContext.Session.GetString("VerificationCode") != null && VerifyCode.Equals(HttpContext.Session.GetString("VerificationCode")))
            {
                if (HttpContext.Session.GetString("TempAccount") != null)
                {
                    RegisterAccount(JsonConvert.DeserializeObject<Account>(HttpContext.Session.GetString("TempAccount")));
                }
            }
            else
            {
                ViewData["Message"] = "Mã xác minh không chính xác";
                return View("VerifyAccount");
            }


            HttpContext.Session.Remove("SignUp");
            HttpContext.Session.Remove("VerificationCode");
            HttpContext.Session.Remove("TempAccount");
            ViewData["Message"] = "Đăng ký thành công. Đăng nhập ngay";
            return View("Index");
        }
    }
}
