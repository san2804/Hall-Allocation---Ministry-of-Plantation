namespace BookingSystem.Api.DTOs;

public class AuthResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}
