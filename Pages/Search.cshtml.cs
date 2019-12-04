using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swrith.Classes;
using Swrith.Utils;

namespace Swrith.Pages
{
    public class SearchModel : PageModel
    {
        [BindProperty]
        public ICollection<Post> TotalPosts { get; set; }
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
            SearchType = type;
            Search = search;
            CurrentPage = pageNumber;
            if (!String.IsNullOrEmpty(search))
            {
                switch (type)
                {
                    case "tag":
                        LoadArticlesByTag(search);
                        break;
                    case "search":
                    default:
                        LoadArticlesBySearch(search.ToLower());
                        break;
                }
            }
        }

        private void LoadArticlesBySearch(string search)
        {
            TotalPosts = PostUtils.TotalPosts.Where(post => post.Title.ToLower().Contains(search)
                        || post.Preview.ToLower().Contains(search)).ToList();
            SetDisplayedPost();
        }

        private void LoadArticlesByTag(string tag)
        {
            TotalPosts = PostUtils.TotalPosts.Where(post => post.Categories.Contains(tag)).ToList();
            SetDisplayedPost();
        }

        private void SetDisplayedPost()
        {
            if (TotalPosts.Any())
            {
                TotalPages = (ushort)(((TotalPosts.Count - 1) / 6) + 1);
                DisplayedPosts = TotalPosts.Skip((CurrentPage - 1) * 6).Take(6).ToList();
            }
            else
                DisplayedPosts = new List<Post>();
        }
    }
}