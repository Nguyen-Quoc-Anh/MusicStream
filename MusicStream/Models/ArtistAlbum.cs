using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class ArtistAlbum
    {
        public string ArtistId { get; set; }
        public string AlbumId { get; set; }

        public virtual Album Album { get; set; }
        public virtual Artist Artist { get; set; }
    }
}
