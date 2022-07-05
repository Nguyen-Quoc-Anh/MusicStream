using System;
using System.Collections.Generic;

#nullable disable

namespace MusicStream.Models
{
    public partial class Comment
    {
        public Comment()
        {
            InverseParent = new HashSet<Comment>();
        }

        public string CommentId { get; set; }
        public string Content { get; set; }
        public string AccountId { get; set; }
        public string TrackId { get; set; }
        public string ParentId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Comment Parent { get; set; }
        public virtual Track Track { get; set; }
        public virtual ICollection<Comment> InverseParent { get; set; }
    }
}
