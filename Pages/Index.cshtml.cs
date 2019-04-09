using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dice_Driven_Stories.Classes;

namespace Dice_Driven_Stories.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ICollection<Post> DisplayedPosts { get; set; }
        [BindProperty]
        public ushort TotalPages { get; set; } = 1;
        [BindProperty]
        public ushort CurrentPage { get; set; } = 1;
        [BindProperty]
        public ushort NextPage { get => (ushort)(CurrentPage + 1); }
        [BindProperty]
        public ushort OldPage { get => (ushort)(CurrentPage - 1); }

        public void OnGet(ushort pageNumber = 1)
        {
            CurrentPage = pageNumber;
            LoadLatestArticles();
        }

        private void LoadLatestArticles()
        {
            TotalPages = (ushort)(((Startup.TotalPosts.Count - 1) / 6) + 1);
            DisplayedPosts = Startup.TotalPosts.Skip((CurrentPage - 1) * 6).Take(6).ToList();
        }
    }
}
