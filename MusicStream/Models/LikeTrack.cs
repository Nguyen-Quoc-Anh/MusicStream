using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class LikeTrack
    {
        public string TrackId { get; set; }
        public string AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Track Track { get; set; }
    }
}
