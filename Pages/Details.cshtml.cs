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
    public class DetailsModel : PageModel
    {
        private readonly Roll_Driven_Stories.Models.RDSContext _context;

        public DetailsModel(Roll_Driven_Stories.Models.RDSContext context)
        {
            _context = context;
        }

        public Post Post { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post = await _context.Post.FirstOrDefaultAsync(m => m.ID == id);

            if (Post == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
