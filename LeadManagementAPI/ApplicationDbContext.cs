using LeadManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LeadManagementAPI.Data
{
    /// <summary>
    /// Main EF Core DbContext for the application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadAttachment> LeadAttachments { get; set; }

        /// <summary>
        /// Configure entity properties and relationships
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER 
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.HasIndex(u => u.Username).IsUnique();

                entity.Property(u => u.Username).HasMaxLength(100).IsRequired();

                entity.Property(u => u.Password).IsRequired();
            });

            // LEAD 
            modelBuilder.Entity<Lead>(entity =>
            {
                entity.HasKey(l => l.LeadId);

                entity.Property(l => l.LeadName).HasMaxLength(200).IsRequired();

                entity.Property(l => l.CompanyName).HasMaxLength(200).IsRequired();

                entity.Property(l => l.Email).HasMaxLength(200).IsRequired();

                entity.Property(l => l.PhoneNumber).HasMaxLength(20).IsRequired();

                entity.Property(l => l.AssignedTo).HasMaxLength(200).IsRequired();

                entity.Property(l => l.Status).HasConversion<string>().HasMaxLength(50);
            });

            // ATTACHMENT 
            modelBuilder.Entity<LeadAttachment>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.FileName).HasMaxLength(500).IsRequired();

                entity.Property(a => a.FilePath).HasMaxLength(1000).IsRequired();

                entity.HasOne(a => a.Lead).WithMany(l => l.Attachments).HasForeignKey(a => a.LeadId).OnDelete(DeleteBehavior.Cascade);
            });

            // STATIC  DATA 
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "Gowtham",
                    Password = "Gowtham@123"
                }
            );
        }
    }
}