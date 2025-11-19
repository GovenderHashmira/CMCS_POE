using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Claims;

namespace CMCS_POE.Models
{
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

            builder.Entity<AppUser>()
                   .Property(u => u.HourlyRate)
                   .HasPrecision(18, 2);

            builder.Entity<Claim>()
                   .Property(c => c.HoursWorked)
                   .HasPrecision(18, 2);

            builder.Entity<Claim>()
                   .Property(c => c.HourlyRate)
                   .HasPrecision(18, 2);

            builder.Entity<Claim>()
                   .HasOne(c => c.Lecturer)          
                   .WithMany()                        
                   .HasForeignKey(c => c.LecturerId) 
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<DocumentUpload>()
                   .HasOne(d => d.Claim)            
                   .WithMany(c => c.DocumentUploads) 
                   .HasForeignKey(d => d.ClaimId)    
                   .OnDelete(DeleteBehavior.Cascade); 

        }
    }
}
