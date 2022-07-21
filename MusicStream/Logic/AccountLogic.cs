using MusicStream.Logic;
using MusicStream.Models;
using System.Linq;
using System;
using System.Collections.Generic;

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

        public static bool ChangeUsername(Account account, string name)
        {
            try
            {
                using (var context = new MusicStreamingContext())
                {
                    account.Fullname = name;
                    context.Accounts.Update(account);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static void ChangeUserAvatar(Account account, string imgPath)
        {
            using (var context = new MusicStreamingContext())
            {
                account.Image = imgPath;
                context.Accounts.Update(account);
                context.SaveChanges();
            }
        }

        public static List<Account> GetAllAccounts()
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.ToList();
            }
        }

        public static bool ActiveAccount(string accountId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    var account = context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                    account.Status = true;
                    context.Accounts.Update(account);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool InactiveAccount(string accountId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    var account = context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                    account.Status = false;
                    context.Accounts.Update(account);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool InsertAccount(Account account)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Accounts.Add(account);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
