using Microsoft.EntityFrameworkCore;
using OfflineTicketingSystemAPI.Helpers;
using OfflineTicketingSystemAPI.Models;

namespace OfflineTicketingSystemAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationship for CreatedByUserId
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship for AssignedToUserId
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedToUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed initial users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("eeba8aae-f6b8-46c9-99cc-05446790868f"),
                    FullName = "Admin User",
                    Email = "admin@test.com",
                    PasswordHash = PasswordHasher.HashPassword("P@s$w0rd123"),
                    Role = Role.Admin
                },
                new User
                {
                    Id = Guid.Parse("2f6f2733-0745-4fe7-9291-91d6c3bc8e39"),
                    FullName = "Employee User",
                    Email = "employee@test.com",
                    PasswordHash = PasswordHasher.HashPassword("P@s$w0rd123"),
                    Role = Role.Employee
                }
            );

            // Seed initial tickets
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    Id = Guid.NewGuid(), // GUID جدید
                    Title = "Fix database connection issue",
                    Description = "The application is unable to connect to the production database.",
                    Status = TicketStatus.Open,
                    Priority = TicketPriority.High,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedByUserId = Guid.Parse("2f6f2733-0745-4fe7-9291-91d6c3bc8e39"), // Id کاربر Employee
                    AssignedToUserId = null
                },
                new Ticket
                {
                    Id = Guid.NewGuid(), // GUID جدید
                    Title = "Add new feature request",
                    Description = "A new feature is required for the user dashboard.",
                    Status = TicketStatus.InProgress,
                    Priority = TicketPriority.Medium,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    UpdatedAt = DateTime.UtcNow,
                    CreatedByUserId = Guid.Parse("2f6f2733-0745-4fe7-9291-91d6c3bc8e39"), // Id کاربر Employee
                    AssignedToUserId = Guid.Parse("eeba8aae-f6b8-46c9-99cc-05446790868f")  // Id کاربر Admin
                }
            );
        }
    }
}
