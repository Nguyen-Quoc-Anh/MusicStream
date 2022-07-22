using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStream.Extensions;
using MusicStream.Logic;
using MusicStream.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicStream.Controllers.Logic
{
    public class ArtistLogic
    {
        public static List<Artist> GetFeatureArtists()
        {
            List<Artist> Artists = new List<Artist>();
            using (var context = new MusicStreamingContext())
            {
                Artists = context.Artists.ToList().PickRandom(10).ToList();
            }
            return (Artists.Count < 10) ? Artists : Artists.Take(10).ToList();
        }

        public static Artist GetArtistByID(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Artists.FirstOrDefault(a => a.ArtistId == Id);
            }
        }

        public static List<Artist> GetAllArtist()
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Artists.ToList();
            }
        }

        public static MultiSelectList GetAllArtistAsMultiSelectList()
        {
            var artists = GetAllArtist();
            var multiSelectList = new MultiSelectList(artists, "ArtistId", "Fullname");
            return multiSelectList;
        }

        public static List<Artist> GetArtistsByName(string name, string sort)
        {
            using (var context = new MusicStreamingContext())
            {
                List<Artist> artists = context.Artists.Where(p => p.Fullname.ToLower().Contains(name.ToLower())).ToList();
                if (sort.Equals("asc"))
                {
                    artists = artists.OrderBy(a => a.Fullname).ToList();
                }
                else
                {
                    artists = artists.OrderByDescending(a => a.Fullname).ToList();
                }
                return artists;
            }
        }

        public static bool CheckArtistIdExtist(string artisrId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Artists.Any(a => a.ArtistId == artisrId);
            }
        }

        public static bool InsertArtist(Artist artist)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Artists.Add(artist);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool UpdateArtist(Artist artist)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Artists.Update(artist);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool DeleteArtist(string artistId, IWebHostEnvironment webHostEnvironment)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.ArtistAlbums.RemoveRange(context.ArtistAlbums.Where(a => a.ArtistId == artistId));
                    context.ArtistTracks.RemoveRange(context.ArtistTracks.Where(a => a.ArtistId == artistId));
                    Artist artist = context.Artists.FirstOrDefault(a => a.ArtistId == artistId);
                    context.Artists.Remove(artist);
                    context.SaveChanges();
                    if (!artist.Image.Contains("http"))
                    {
                        Util.DeleteFile(webHostEnvironment, artist.Image.Split('/').Last(), "img/artist");
                    }
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
