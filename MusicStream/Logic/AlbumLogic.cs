using Microsoft.EntityFrameworkCore;
using MusicStream.Models;
using System.Collections.Generic;
using System.Linq;

namespace MusicStream.Controllers.Logic
{
    public class AlbumLogic
    {
        public static Dictionary<Album, List<Account>> GetFeatureAlbum()
        {
            Dictionary<Album, List<Account>> albums = new Dictionary<Album, List<Account>>();
            using (var context = new MusicStreamingContext())
            {
                List<Album> albumList = context.Albums.OrderByDescending(al => al.ReleaseDate).Take(12).ToList();

                albumList.ForEach(album =>
                {
                    List<Account> Accounts = new List<Account>();
                    Accounts = context.Accounts.Join(context.ArtistAlbums.Where(at => at.AlbumId == album.AlbumId),
                        t => t.AccountId,
                        a => a.AccountId,
                        (t, a) => t
                        ).ToList();
                    albums.Add(album, Accounts);
                });
            }
            return albums;
        }

        public static Dictionary<Album, List<Account>> GetAlbumDetails(string id)
        {
            Dictionary<Album, List<Account>> albums = new Dictionary<Album, List<Account>>();
            using (var context = new MusicStreamingContext())
            {
                Album album = context.Albums.FirstOrDefault(a => a.AlbumId.Equals(id));
                if (album == null) return null;

                List<Account> Accounts = new List<Account>();
                Accounts = context.Accounts.Join(context.ArtistAlbums.Where(at => at.AlbumId == album.AlbumId),
                    t => t.AccountId,
                    a => a.AccountId,
                    (t, a) => t
                    ).ToList();
                albums.Add(album, Accounts);
            }
            return albums;
        }

        public static Dictionary<Album, List<Account>> GetRandomAlbum(int number)
        {
            Dictionary<Album, List<Account>> albums = new Dictionary<Album, List<Account>>();
            using (var context = new MusicStreamingContext())
            {
                List<Album> albumList = context.Albums.ToList().PickRandom(number).ToList();

                albumList.ForEach(album =>
                {
                    List<Account> Accounts = new List<Account>();
                    Accounts = context.Accounts.Join(context.ArtistAlbums.Where(at => at.AlbumId == album.AlbumId),
                        t => t.AccountId,
                        a => a.AccountId,
                        (t, a) => t
                        ).ToList();
                    albums.Add(album, Accounts);
                });
            }
            return albums;
        }

        public static List<Album> GetAlbumByArtistId(string id)
        {
            using (var context = new MusicStreamingContext())
            {
                List<Album> albumList = context.Albums.Include(a => a.ArtistAlbums).ThenInclude(a => a.Account)
                    .Where(a => a.ArtistAlbums.Any(al => al.AccountId == id)).ToList();
                return albumList;
            }
        }
    }
}
