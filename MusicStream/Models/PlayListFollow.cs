using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class PlayListFollow
    {
        public string AccountId { get; set; }
        public string PlaylistId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Playlist Playlist { get; set; }
    }
}
