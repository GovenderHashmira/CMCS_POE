using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMCS_POE.Models
{
    // ApplicationDbContext inherits from IdentityDbContext<AppUser>
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public DbSet<Claim> Claims { get; set; }
        //public DbSet<DocumentUpload> DocumentUploads { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Specify precision and scale for HourlyRate
            builder.Entity<AppUser>()
                   .Property(u => u.HourlyRate)
                   .HasPrecision(18, 2); // 18 digits total, 2 decimal places
        }
    }
}
