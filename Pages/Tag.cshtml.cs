using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Roll_Driven_Stories.Classes;

namespace Roll_Driven_Stories.Pages
{
    public class TagModel : PageModel
    {
        [BindProperty]
        public ICollection<Post> DisplayedPosts { get; set; }
        [BindProperty]
        public ICollection<Post> TotalPosts { get; set; }
        public ushort PageIndex { get; set; }
        public void OnGet(string tag)
        {
            LoadArticlesByTag(tag);
        }

        private void LoadArticlesByTag(string tag)
        {
            
            TotalPosts = Startup.TotalPosts.Where(post => post.Categories.Contains(tag)).ToList();
            if(TotalPosts.Any())
                DisplayedPosts = TotalPosts.Take(6).ToList();
            else
                DisplayedPosts = new List<Post>();
        }
        private void ChangeDisplayedArticlesByPage()
        {
            DisplayedPosts = TotalPosts.Skip((PageIndex) * 6).Take(6).ToList();
        }
    }
}
