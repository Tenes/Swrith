using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Roll_Driven_Stories.Models;

namespace Roll_Driven_Stories.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly Roll_Driven_Stories.Models.RDSContext _context;

        public IndexModel(Roll_Driven_Stories.Models.RDSContext context)
        {
            _context = context;
        }

        public IList<Post> Post { get;set; }

        public async Task OnGetAsync()
        {
            Post = await _context.Post.ToListAsync();
        }
    }
}
