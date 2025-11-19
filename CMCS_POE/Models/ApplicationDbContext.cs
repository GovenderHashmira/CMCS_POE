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

        public DbSet<Claim> Claims { get; set; }
        public DbSet<DocumentUpload> DocumentUploads { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Specify precision for HourlyRate in AppUser
            builder.Entity<AppUser>()
                   .Property(u => u.HourlyRate)
                   .HasPrecision(18, 2);

            // Specify precision for Claim HoursWorked and HourlyRate
            builder.Entity<Claim>()
                   .Property(c => c.HoursWorked)
                   .HasPrecision(18, 2);

            builder.Entity<Claim>()
                   .Property(c => c.HourlyRate)
                   .HasPrecision(18, 2);
        }

    }
}
