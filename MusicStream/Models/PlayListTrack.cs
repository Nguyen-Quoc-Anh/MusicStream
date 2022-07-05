using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class PlayListTrack
    {
        public string PlaylistId { get; set; }
        public string TrackId { get; set; }

        public virtual Playlist Playlist { get; set; }
        public virtual Track Track { get; set; }
    }
}
