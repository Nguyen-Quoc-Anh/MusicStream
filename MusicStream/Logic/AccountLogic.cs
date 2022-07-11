using MusicStream.Logic;
using MusicStream.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace MusicStream.Controllers.Logic
{
    public class AccountLogic
    {
        public static bool IsEmailAlreadyInUse(string email)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.Any(a => a.Email == email);
            }
        }

        public static bool IsIdExist(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.Any(a => a.AccountId == Id);
            }
        }

        public static void RegisterAccount(Account a)
        {
            using (var context = new MusicStreamingContext())
            {
                context.Accounts.Add(a);
                context.SaveChanges();
            }
        }

        public static Account SignIn(string Email, string Password)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.FirstOrDefault(a => a.Email.Equals(Email) && Util.EncodePassword(Password).Equals(a.Password));
            }
        }

        public static Account SignInWithGoogle(string Email, string AccountId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.FirstOrDefault(a => a.Email.Equals(Email) && a.AccountId == AccountId);
            }
        }

        public static Account GetAccountById(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.FirstOrDefault(a => a.AccountId == Id);
            }
        }

        public static bool ChangePassword(string accountId, string password)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    var account = new Account { AccountId = accountId, Password = password };
                    context.Accounts.Attach(account).Property(x => x.Password).IsModified = true;
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static string GetAccountIdByEmail(string Email)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.FirstOrDefault(a => a.Email == Email).AccountId;
            }
        }
    }
}
