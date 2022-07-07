using Microsoft.AspNetCore.Http;
using MusicStream.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static MusicStream.Controllers.Logic.AccountLogic;

namespace MusicStream.Logic
{
    public class Util
    {
        private static Random random = new Random();

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
    }
}
