using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Account
    {
        public Account()
        {
            Comments = new HashSet<Comment>();
            LikeTracks = new HashSet<LikeTrack>();
            PlayListFollows = new HashSet<PlayListFollow>();
            Playlists = new HashSet<Playlist>();
        }

        public string AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public string Fullname { get; set; }
        public string Image { get; set; }
        public bool? Status { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<LikeTrack> LikeTracks { get; set; }
        public virtual ICollection<PlayListFollow> PlayListFollows { get; set; }
        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}
