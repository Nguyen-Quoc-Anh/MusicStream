using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Comment
    {
        public string CommentId { get; set; }
        public string Content { get; set; }
        public string AccountId { get; set; }
        public string AlbumId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual Account Account { get; set; }
        public virtual Album Album { get; set; }
    }
}
