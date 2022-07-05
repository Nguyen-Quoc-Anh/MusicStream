using MusicStream.Models;
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
    }
}
