using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Genre
    {
        public Genre()
        {
            AlbumGenres = new HashSet<AlbumGenre>();
            GenreOfTracks = new HashSet<GenreOfTrack>();
        }

        public string GenreId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AlbumGenre> AlbumGenres { get; set; }
        public virtual ICollection<GenreOfTrack> GenreOfTracks { get; set; }
    }
}
