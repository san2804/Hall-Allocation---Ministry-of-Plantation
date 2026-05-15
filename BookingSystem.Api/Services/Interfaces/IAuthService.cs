using BookingSystem.Api.DTOs;

namespace BookingSystem.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
