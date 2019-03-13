using Microsoft.EntityFrameworkCore;

namespace Roll_Driven_Stories.Models
{
    public class RDSContext : DbContext
    {
        public RDSContext (DbContextOptions<RDSContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Post { get; set; }
    }
}