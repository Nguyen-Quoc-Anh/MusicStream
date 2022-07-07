using Microsoft.EntityFrameworkCore;
using MusicStream.Models;
using System.Collections.Generic;
using System.Linq;

namespace MusicStream.Controllers.Logic
{
    public class AlbumLogic
    {
        public static Dictionary<Album, List<Artist>> GetFeatureAlbum()
        {
            Dictionary<Album, List<Artist>> albums = new Dictionary<Album, List<Artist>>();
            using (var context = new MusicStreamingContext())
            {
                List<Album> albumList = context.Albums.OrderByDescending(al => al.ReleaseDate).Take(12).ToList();

                albumList.ForEach(album =>
                {
                    List<Artist> Artists = new List<Artist>();
                    Artists = context.Artists.Join(context.ArtistAlbums.Where(at => at.AlbumId == album.AlbumId),
                        t => t.ArtistId,
                        a => a.ArtistId,
                        (t, a) => t
                        ).ToList();
                    albums.Add(album, Artists);
                });
            }
            return albums;
        }

        public static Album GetAlbumDetails(string id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Albums.Include(a => a.Tracks).ThenInclude(a => a.ArtistTracks).ThenInclude(a => a.Artist)
                    .Include(a => a.ArtistAlbums).ThenInclude(a => a.Artist).FirstOrDefault(a => a.AlbumId == id);
            }
        }

        public static Dictionary<Album, List<Artist>> GetRandomAlbum(int number)
        {
            Dictionary<Album, List<Artist>> albums = new Dictionary<Album, List<Artist>>();
            using (var context = new MusicStreamingContext())
            {
                List<Album> albumList = context.Albums.ToList().PickRandom(number).ToList();

                albumList.ForEach(album =>
                {
                    List<Artist> Artists = new List<Artist>();
                    Artists = context.Artists.Join(context.ArtistAlbums.Where(at => at.AlbumId == album.AlbumId),
                        t => t.ArtistId,
                        a => a.ArtistId,
                        (t, a) => t
                        ).ToList();
                    albums.Add(album, Artists);
                });
            }
            return albums;
        }

        public static List<Album> GetAlbumByArtistId(string id)
        {
            using (var context = new MusicStreamingContext())
            {
                List<Album> albumList = context.Albums.Include(a => a.ArtistAlbums).ThenInclude(a => a.Artist)
                    .Where(a => a.ArtistAlbums.Any(al => al.ArtistId == id)).ToList();
                return albumList;
            }
        }
    }
}
