using MusicStream.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicStream.Controllers.Logic
{
    public class ArtistLogic
    {
        public static List<Account> GetFeatureArtists()
        {
            List<Account> Accounts = new List<Account>();
            using (var context = new MusicStreamingContext())
            {
                Accounts = context.Accounts.ToList().PickRandom(10).ToList();
            }
            return (Accounts.Count < 10) ? Accounts : Accounts.Take(10).ToList();
        }

        public static Account GetArtistByID(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.FirstOrDefault(a => a.AccountId == Id);
            }
        }

        public static List<Account> GetAllArtist()
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Accounts.Where(a => a.RoleId == 2).ToList();
            }
        }
    }

    public static class Extensions
    {
        static Random rnd = new Random();

        public static IEnumerable<T> PickRandom<T>(this IList<T> source, int count)
        {
            return source.OrderBy(x => rnd.Next()).Take(count);
        }
    }
}
