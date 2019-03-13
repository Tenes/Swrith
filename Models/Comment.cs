using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Roll_Driven_Stories.Models
{
    public class Comment
    {
        [Required]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Author { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public bool IsAdmin { get; set; }

        public string RenderContent()
        {
            return Content;
        }
    }
}