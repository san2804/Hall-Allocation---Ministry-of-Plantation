using BookingSystem.Data.Entities;

namespace BookingSystem.Api.Services.Interfaces;

public interface IHallService
{
    Task<IEnumerable<Hall>> GetAllAsync();
    Task<Hall?> GetByIdAsync(int id);
    Task<Hall> CreateAsync(Hall hall);
    Task<bool> UpdateAsync(Hall hall);
    Task<bool> DeleteAsync(int id);
}
