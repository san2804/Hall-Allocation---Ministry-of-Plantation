using BookingSystem.Api.Services.Interfaces;
using BookingSystem.Data;
using BookingSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Services;

public class HallService : IHallService
{
    private readonly AppDbContext _context;

    public HallService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Hall>> GetAllAsync()
    {
        return await _context.Halls.ToListAsync();
    }

    public async Task<Hall?> GetByIdAsync(int id)
    {
        return await _context.Halls.FindAsync(id);
    }

    public async Task<Hall> CreateAsync(Hall hall)
    {
        _context.Halls.Add(hall);
        await _context.SaveChangesAsync();
        return hall;
    }

    public async Task<bool> UpdateAsync(Hall hall)
    {
        _context.Entry(hall).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hall = await _context.Halls.FindAsync(id);
        if (hall == null) return false;

        _context.Halls.Remove(hall);
        await _context.SaveChangesAsync();
        return true;
    }
}
