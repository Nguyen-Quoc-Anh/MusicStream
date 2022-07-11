using MusicStream.Models;
using System;
using System.Linq;

namespace MusicStream.Logic
{
    public class CommentLogic
    {
        public static bool AddComment(string accountId, string content, string albumId, string commentId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    Comment comment = new Comment() { AccountId = accountId, Content = content, AlbumId = albumId, CommentId = commentId };
                    context.Comments.Add(comment);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool EditComment(string content, string commentId)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    Comment comment = GetCommentById(commentId);
                    comment.Content = content;
                    comment.UpdateTime = DateTime.Now;
                    context.Comments.Update(comment);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool CheckCommentExistById(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Comments.Any(c => c.CommentId == Id);
            }
        }

        public static bool DeleteCommentById(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                try
                {
                    context.Comments.Remove(GetCommentById(Id));
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static Comment GetCommentById(string Id)
        {
            using (var context = new MusicStreamingContext())
            {
                return context.Comments.FirstOrDefault(c => c.CommentId.Equals(Id));
            }
        }
    }
}
