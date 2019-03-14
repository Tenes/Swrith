using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Roll_Driven_Stories.Pages
{
    public class ArticleModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }
        public void OnGet()
        {
            
        }
    }
}
