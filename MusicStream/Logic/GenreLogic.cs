using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStream.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicStream.Controllers.Logic
{
    public class GenreLogic
    {
        public static List<Genre> GetAllGenres()
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Genres.ToList();
            }
        }

        public static MultiSelectList GetAllGenresAsMultiSelectList()
        {
            var genres = GetAllGenres();
            var multiSelectList = new MultiSelectList(genres, "GenreId", "Name");
            return multiSelectList;
        }

        public static bool CheckGenreExistById(string genreId)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Genres.Any(g => g.GenreId == genreId);
            }
        }

        public static bool DeleteGenreById(string genreId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.AlbumGenres.RemoveRange(context.AlbumGenres.Where(g => g.GenreId == genreId));
                    context.GenreOfTracks.RemoveRange(context.GenreOfTracks.Where(g => g.GenreId == genreId));
                    context.Genres.Remove(context.Genres.FirstOrDefault(g => g.GenreId == genreId));
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool AddNewGenre(Genre genre)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Genres.Add(genre);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool UpdateGenre(Genre genre)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Genres.Update(genre);
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
