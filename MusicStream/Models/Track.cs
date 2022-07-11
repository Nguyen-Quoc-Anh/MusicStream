using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Track
    {
        public Track()
        {
            ArtistTracks = new HashSet<ArtistTrack>();
            GenreOfTracks = new HashSet<GenreOfTrack>();
            LikeTracks = new HashSet<LikeTrack>();
            PlayListTracks = new HashSet<PlayListTrack>();
        }

        public string TrackId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int? Duration { get; set; }
        public string Mp3 { get; set; }
        public string AlbumId { get; set; }
        public int? View { get; set; }

        public virtual Album Album { get; set; }
        public virtual ICollection<ArtistTrack> ArtistTracks { get; set; }
        public virtual ICollection<GenreOfTrack> GenreOfTracks { get; set; }
        public virtual ICollection<LikeTrack> LikeTracks { get; set; }
        public virtual ICollection<PlayListTrack> PlayListTracks { get; set; }
    }
}
