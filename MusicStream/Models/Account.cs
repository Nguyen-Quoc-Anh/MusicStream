using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Account
    {
        public Account()
        {
            ArtistAlbums = new HashSet<ArtistAlbum>();
            ArtistTracks = new HashSet<ArtistTrack>();
            Comments = new HashSet<Comment>();
            LikeTracks = new HashSet<LikeTrack>();
            Playlists = new HashSet<Playlist>();
        }

        public string AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<ArtistAlbum> ArtistAlbums { get; set; }
        public virtual ICollection<ArtistTrack> ArtistTracks { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<LikeTrack> LikeTracks { get; set; }
        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}
