using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Models;

namespace dotnetapp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CookingClass> CookingClasses { get; set; }
        public DbSet<CookingClassRequest> CookingClassRequests { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public new DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // MUST call base to configure Identity tables
            base.OnModelCreating(modelBuilder);

            // Map custom User to separate table
            modelBuilder.Entity<User>().ToTable("Users");

            // Decimal precision for Fee
            modelBuilder.Entity<CookingClass>()
                .Property(c => c.Fee)
                .HasColumnType("decimal(18,2)");

            // CookingClassRequest relationships
            modelBuilder.Entity<CookingClassRequest>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CookingClassRequest>()
                .HasOne(r => r.CookingClass)
                .WithMany()
                .HasForeignKey(r => r.CookingClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback relationship
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}