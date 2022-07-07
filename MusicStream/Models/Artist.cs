using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Artist
    {
        public Artist()
        {
            ArtistAlbums = new HashSet<ArtistAlbum>();
            ArtistTracks = new HashSet<ArtistTrack>();
        }

        public string ArtistId { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ArtistAlbum> ArtistAlbums { get; set; }
        public virtual ICollection<ArtistTrack> ArtistTracks { get; set; }
    }
}
