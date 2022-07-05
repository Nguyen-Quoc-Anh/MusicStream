using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class AlbumGenre
    {
        public string GenreId { get; set; }
        public string AlbumId { get; set; }

        public virtual Album Album { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
