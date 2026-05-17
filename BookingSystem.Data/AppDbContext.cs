using BookingSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Hall> Halls { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Unique username
        builder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        builder.Entity<Booking>()
            .HasOne(b => b.Hall)
            .WithMany()
            .HasForeignKey(b => b.HallId);

        builder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId);
    }
}
