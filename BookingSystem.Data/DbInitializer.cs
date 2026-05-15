using BookingSystem.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace BookingSystem.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var hasher = new PasswordHasher<User>();

        // Create Admin user
        var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
        if (admin == null)
        {
            admin = new User
            {
                Username = "admin",
                FullName = "Admin User",
                Role = "Admin"
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
            context.Users.Add(admin);
        }
        else
        {
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
        }

        // Create a normal user
        var officer = context.Users.FirstOrDefault(u => u.Username == "officer");
        if (officer == null)
        {
            officer = new User
            {
                Username = "officer",
                FullName = "Officer User",
                Role = "Officer"
            };
            officer.PasswordHash = hasher.HashPassword(officer, "Officer123!");
            context.Users.Add(officer);
        }
        else
        {
            officer.PasswordHash = hasher.HashPassword(officer, "Officer123!");
        }

        // Seed ONLY the single Ministry Conference Hall
        if (!context.Halls.Any())
        {
            context.Halls.Add(new Hall 
            { 
                Name = "Ministry Conference Hall", 
                Capacity = 50, 
                Location = "Ministry of Plantation, Ground Floor", 
                Facilities = "Projector, Sound System, WiFi, Whiteboard, Air Conditioned" 
            });
        }

        await context.SaveChangesAsync();
    }
}
