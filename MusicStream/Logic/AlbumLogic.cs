using Microsoft.EntityFrameworkCore;
using MusicStream.Extensions;
using MusicStream.Models;
using System;
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

        public static Album GetAlbumDetails(string id, string accountId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Albums.Include(a => a.Tracks).ThenInclude(a => a.ArtistTracks).ThenInclude(a => a.Artist)
                    .Include(a => a.Tracks).ThenInclude(a => a.LikeTracks.Where(lt => lt.AccountId == accountId))
                    .Include(a => a.ArtistAlbums).ThenInclude(a => a.Artist)
                    .Include(a => a.Comments).ThenInclude(a => a.Account)
                    .FirstOrDefault(a => a.AlbumId == id);
            }
        }

        public static Album GetAlbumDetails(string id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Albums.Include(a => a.Tracks).ThenInclude(a => a.ArtistTracks).ThenInclude(a => a.Artist)
                    .Include(a => a.ArtistAlbums).ThenInclude(a => a.Artist)
                    .Include(a => a.Comments).ThenInclude(a => a.Account)
                    .FirstOrDefault(a => a.AlbumId == id);
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

        public static List<Album> GetAllAlbum()
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Albums.ToList();
            }
        }

        public static void ModifyAlbumGenre(string albumId, string[] genres)
        {
            using (var context = new MusicStreamingContext())
            {
                Album album = context.Albums.FirstOrDefault(a => a.AlbumId == albumId);
                if (album != null)
                {
                    genres.ToList().ForEach(genre =>
                    {
                        AlbumGenre albumGenre = context.AlbumGenres.FirstOrDefault(a => a.AlbumId == albumId && a.GenreId == genre);
                        if (albumGenre == null)
                        {
                            albumGenre = new AlbumGenre()
                            {
                                AlbumId = albumId,
                                GenreId = genre
                            };
                            context.AlbumGenres.Add(albumGenre);
                        }
                    });
                    context.SaveChanges();
                }
            }
        }

        public static void ModifyAlbumArtist(string albumId, string[] artists)
        {
            using (var context = new MusicStreamingContext())
            {
                Album album = context.Albums.FirstOrDefault(a => a.AlbumId == albumId);
                if (album != null)
                {
                    artists.ToList().ForEach(artist =>
                    {
                        ArtistAlbum artistAlbum = context.ArtistAlbums.FirstOrDefault(a => a.AlbumId == albumId && a.ArtistId == artist);
                        if (artistAlbum == null)
                        {
                            artistAlbum = new ArtistAlbum()
                            {
                                AlbumId = albumId,
                                ArtistId = artist
                            };
                            context.ArtistAlbums.Add(artistAlbum);
                        }
                    });
                    context.SaveChanges();
                }
            }
        }

        public static bool InsertAlbum(Album album)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Albums.Add(album);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool DeleteAlbum(string albumId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Tracks.Where(t => t.AlbumId == albumId).ToList().ForEach(t =>
                    {
                        bool success = TrackLogic.DeleteTrack(t.TrackId);
                    });
                    context.ArtistAlbums.RemoveRange(context.ArtistAlbums.Where(a => a.AlbumId == albumId));
                    context.AlbumGenres.RemoveRange(context.AlbumGenres.Where(a => a.AlbumId == albumId));
                    context.Albums.Remove(context.Albums.FirstOrDefault(a => a.AlbumId == albumId));
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool UpdateAlbum(Album album)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Albums.Update(album);
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
