using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MusicStream.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static MusicStream.Controllers.Logic.AccountLogic;

namespace MusicStream.Logic
{
    public class Util
    {
        private static Random random = new Random();
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;
        public static string CalculateTimeAgo(DateTime time)
        {

            var ts = new TimeSpan(DateTime.Now.Ticks - time.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "một giây trước" : ts.Seconds + " giây trước";

            if (delta < 2 * MINUTE)
                return "một phút trước";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " phút trước";

            if (delta < 90 * MINUTE)
                return "một giờ trước";

            if (delta < 24 * HOUR)
                return ts.Hours + " giờ trước";

            if (delta < 48 * HOUR)
                return "hôm qua";

            if (delta < 30 * DAY)
                return ts.Days + " ngày trước";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "một tháng trước" : months + " tháng trước";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "một năm trước" : years + " năm trước";
            }
        }
        public static string RandomString(int length)
        {
            const string chars = "abcdefghiklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        public static Account CheckLogged(HttpContext context, HttpRequest request)
        {
            if (context.Session.GetString("account") == null)
            {
                if (request.Cookies["account"] == null)
                {
                    Account account = GetAccountById(request.Cookies["account"]);
                    if (account != null)
                    {
                        context.Session.SetString("account", JsonConvert.SerializeObject(account));
                    }
                    return account;
                }
                return null;
            }
            return JsonConvert.DeserializeObject<Account>(context.Session.GetString("account"));
        }

        public static async Task<string> UploadedFile(IFormFile model, IWebHostEnvironment webHostEnvironment, string fileName, string fp)
        {
            if (model != null)
            {//""
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, fp);
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CopyToAsync(fileStream);
                    return $"/{fp}{fileName}";
                }
            }
            return null;
        }

        public static bool DeleteFile(IWebHostEnvironment webHostEnvironment, string fileName, string fp)
        {
            try
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, fp);
                string filePath = Path.Combine(uploadsFolder, fileName);
                File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
