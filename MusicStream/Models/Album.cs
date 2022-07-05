using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Album
    {
        public Album()
        {
            AlbumGenres = new HashSet<AlbumGenre>();
            ArtistAlbums = new HashSet<ArtistAlbum>();
            Tracks = new HashSet<Track>();
        }

        public string AlbumId { get; set; }
        public string AlbumName { get; set; }
        public string Image { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? View { get; set; }

        public virtual ICollection<AlbumGenre> AlbumGenres { get; set; }
        public virtual ICollection<ArtistAlbum> ArtistAlbums { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
