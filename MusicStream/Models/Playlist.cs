using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Playlist
    {
        public Playlist()
        {
            PlayListFollows = new HashSet<PlayListFollow>();
            PlayListTracks = new HashSet<PlayListTrack>();
        }

        public string PlaylistId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string AccountId { get; set; }
        public bool? IsPrivate { get; set; }
        public DateTime? CreatedTime { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<PlayListFollow> PlayListFollows { get; set; }
        public virtual ICollection<PlayListTrack> PlayListTracks { get; set; }
    }
}
