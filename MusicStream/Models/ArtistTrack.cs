using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class ArtistTrack
    {
        public string ArtistId { get; set; }
        public string TrackId { get; set; }

        public virtual Artist Artist { get; set; }
        public virtual Track Track { get; set; }
    }
}
