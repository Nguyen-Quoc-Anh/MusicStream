using MusicStream.Logic;
using MusicStream.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


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

        public static Account GetAccountById(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.FirstOrDefault(a => a.AccountId == Id);
            }
        }
    }
}
