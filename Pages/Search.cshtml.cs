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
using Dice_Driven_Stories.Classes;

namespace Dice_Driven_Stories.Pages
{
    public class SearchModel : PageModel
    {
        [BindProperty]
        public ICollection<Post> DisplayedPosts { get; set; }
        [BindProperty]
        public ushort TotalPages { get; set; } = 1;
        [BindProperty]
        public ushort CurrentPage { get; set; } = 1;
        [BindProperty]
        public string SearchType { get; set; }
        [BindProperty]
        public string Search { get; set; }
        [BindProperty]
        public ushort NextPage { get => (ushort)(CurrentPage + 1); }
        [BindProperty]
        public ushort OldPage { get => (ushort)(CurrentPage - 1); }

        public void OnGet(string type, string search, ushort pageNumber = 1)
        {
            SearchType = search;
            CurrentPage = pageNumber;
            if (!String.IsNullOrEmpty(search))
            {
                switch (type)
                {
                    case "search":
                        LoadArticlesBySearch(search.ToLower());
                        break;
                    case "tag":
                        LoadArticlesByTag(search);
                        break;
                    default:
                        LoadEmptyArticle();
                        break;
                }
            }
            else
                LoadEmptyArticle();
        }

        private void LoadArticlesBySearch(string search)
        {
            var totalPosts = Startup.TotalPosts.Where(post => post.Title.ToLower().Contains(search)
                        || post.Preview.ToLower().Contains(search)).ToList();
            SetDisplayedPost(totalPosts);
        }

        private void LoadArticlesByTag(string tag)
        {
            var totalPosts = Startup.TotalPosts.Where(post => post.Categories.Contains(tag)).ToList();
            SetDisplayedPost(totalPosts);
        }

        private void SetDisplayedPost(ICollection<Post> totalPosts)
        {
            if (totalPosts.Any())
            {
                TotalPages = (ushort)(((totalPosts.Count - 1) / 6) + 1);
                DisplayedPosts = totalPosts.Skip((CurrentPage - 1) * 6).Take(6).ToList();
            }
            else
                DisplayedPosts = new List<Post>();
        }

        private void LoadEmptyArticle()
        {
        }
    }
}
