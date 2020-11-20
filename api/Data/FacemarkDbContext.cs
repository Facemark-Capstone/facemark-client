using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using shared.Models;
using shared.Models.AI;

#nullable disable

namespace api.Data
{
    public partial class FacemarkDbContext : IdentityDbContext<User>
    {
        public DbSet<Order> Orders { get; set; }

        public FacemarkDbContext(DbContextOptions<FacemarkDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
