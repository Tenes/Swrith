using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Roll_Driven_Stories.Extensions;

namespace Roll_Driven_Stories.Models
{
    public class Post
    {
        [Required]
        public string ID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Excerpt { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = true;

        public string Categories { get; set; } = "";

        public IList<Comment> Comments { get; } = new List<Comment>();

        public string GetLink()
        {
            return $"/blog/{Slug}/";
        }

        public string GetEncodedLink()
        {
            return $"/blog/{System.Net.WebUtility.UrlEncode(Slug)}/";
        }

        public bool AreCommentsOpen(int commentsCloseAfterDays)
        {
            return PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;
        }

        public static string CreateSlug(string title)
        {
            return title.ToLowerInvariant()
                .Replace(" ", "-")
                .RemoveDiacritics()
                .RemoveReservedUrlCharacters()
                .ToLowerInvariant();
        }

        

    }
}