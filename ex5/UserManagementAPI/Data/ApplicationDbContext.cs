using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Models;

namespace UserManagementAPI.Data
{
    /// <summary>
    /// Entity Framework Core database context for the application
    /// Manages User entities and database configuration
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Users DbSet representing the Users table
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Configures entity mappings and constraints using Fluent API
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                // Set primary key
                entity.HasKey(u => u.Id);

                // Configure Name field
                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configure Email field with unique constraint
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                // Configure optional fields
                entity.Property(u => u.Phone)
                    .HasMaxLength(20);

                entity.Property(u => u.Website)
                    .HasMaxLength(255);

                entity.Property(u => u.Company)
                    .HasMaxLength(255);

                // Configure timestamp fields
                entity.Property(u => u.CreatedAt)
                    .IsRequired();

                entity.Property(u => u.UpdatedAt)
                    .IsRequired();
            });
        }
    }
}
