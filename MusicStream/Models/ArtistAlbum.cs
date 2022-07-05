using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class ArtistAlbum
    {
        public string AccountId { get; set; }
        public string AlbumId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Album Album { get; set; }
    }
}
