using Microsoft.AspNetCore.Mvc.Rendering;
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
