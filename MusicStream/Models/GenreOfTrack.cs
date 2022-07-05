using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class GenreOfTrack
    {
        public string GenreId { get; set; }
        public string TrackId { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual Track Track { get; set; }
    }
}
