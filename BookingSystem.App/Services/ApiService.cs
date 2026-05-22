using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookingSystem.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private static string? _token;
    private static bool _isAdmin;
    private static string _baseUrl = "http://localhost:5235/api/";

    static ApiService()
    {
        try
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("ApiSettings", out var apiSettings) &&
                    apiSettings.TryGetProperty("BaseUrl", out var url))
                {
                    _baseUrl = url.GetString() ?? _baseUrl;
                }
            }
        }
        catch { /* Fallback to default */ }
    }

    public ApiService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        if (!string.IsNullOrEmpty(_token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
    }

    public bool IsAdmin => _isAdmin;

    public async Task<AuthResponse?> LoginAsync(string username, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", new { Username = username, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null && result.Success)
                {
                    _token = result.Token;
                    _isAdmin = result.IsAdmin;
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }
                return result;
            }
            return new AuthResponse { Success = false, Message = "Invalid credentials." };
        }
        catch
        {
            return new AuthResponse { Success = false, Message = "Could not connect to the server." };
        }
    }

    // Dashboard
    public async Task<DashboardStats?> GetDashboardStatsAsync()
    {
        return await GetAsync<DashboardStats>("dashboard/stats");
    }

    public async Task<ReportStats?> GetReportStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        string url = "dashboard/reports";
        if (fromDate.HasValue || toDate.HasValue)
        {
            url += "?";
            if (fromDate.HasValue) url += $"fromDate={fromDate.Value:O}&";
            if (toDate.HasValue) url += $"toDate={toDate.Value:O}";
        }
        return await GetAsync<ReportStats>(url);
    }

    public async Task<byte[]?> ExportReportToExcelAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        string url = "dashboard/export/excel";
        if (fromDate.HasValue || toDate.HasValue)
        {
            url += "?";
            if (fromDate.HasValue) url += $"fromDate={fromDate.Value:O}&";
            if (toDate.HasValue) url += $"toDate={toDate.Value:O}";
        }
        
        try
        {
            return await _httpClient.GetByteArrayAsync(url);
        }
        catch
        {
            return null;
        }
    }

    // Halls
    public async Task<List<HallResponse>?> GetHallsAsync()
    {
        return await GetAsync<List<HallResponse>>("halls");
    }

    // Bookings
    public async Task<List<BookingResponse>?> GetMyBookingsAsync()
    {
        return await GetAsync<List<BookingResponse>>("bookings/my");
    }

    public async Task<List<BookingResponse>?> GetAllBookingsAsync()
    {
        return await GetAsync<List<BookingResponse>>("bookings/all");
    }

    public async Task<List<BookingResponse>?> GetBookingsByHallAsync(int hallId, DateTime start, DateTime end)
    {
        return await GetAsync<List<BookingResponse>>($"bookings/hall/{hallId}?start={start:O}&end={end:O}");
    }

    public async Task<(bool Success, string Message)> CreateBookingAsync(BookingRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("bookings", request);
        if (response.IsSuccessStatusCode) return (true, "Booking submitted successfully.");
        
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        return (false, error?.Message ?? "Failed to submit booking.");
    }

    public async Task<bool> UpdateBookingStatusAsync(int bookingId, int status, string? remark = null)
    {
        var response = await _httpClient.PutAsJsonAsync($"bookings/{bookingId}/status", new { Status = status, AdminRemark = remark });
        return response.IsSuccessStatusCode;
    }

    // Users
    public async Task<List<UserDto>?> GetUsersAsync()
    {
        return await GetAsync<List<UserDto>>("users");
    }

    public async Task<UserDto?> GetMyProfileAsync()
    {
        return await GetAsync<UserDto>("users/me");
    }

    public async Task<bool> UpdateMyProfileAsync(UpdateUserRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("users/me", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<(bool Success, string Message)> CreateUserAsync(CreateUserRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("users", request);
        if (response.IsSuccessStatusCode) return (true, "User created successfully.");

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        return (false, error?.Message ?? "Failed to create user.");
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"users/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"users/{id}");
        return response.IsSuccessStatusCode;
    }

    public void Logout()
    {
        _token = null;
        _isAdmin = false;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<T>(endpoint);
        }
        catch
        {
            return default;
        }
    }
}

public class ApiErrorResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = "";
}

// DTOs for the App (Simplified)
public class AuthResponse 
{ 
    [JsonPropertyName("success")]
    public bool Success { get; set; } 
    
    [JsonPropertyName("token")]
    public string Token { get; set; } = ""; 
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = ""; 
    
    [JsonPropertyName("isAdmin")]
    public bool IsAdmin { get; set; } 
    
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = "";
    
    [JsonPropertyName("role")]
    public string Role { get; set; } = "";
}
public class DashboardStats { public int TotalBookings { get; set; } public int UpcomingBookings { get; set; } public int PendingApprovals { get; set; } public int ThisMonthBookings { get; set; } }

public class ReportStats
{
    public int TotalBookings { get; set; }
    public int ApprovedBookings { get; set; }
    public int PendingBookings { get; set; }
    public int RejectedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public List<PurposeStat> ByPurpose { get; set; } = new();
    public List<MonthlyOccupancyStat> MonthlyOccupancy { get; set; } = new();
}

public class PurposeStat
{
    public string Purpose { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class MonthlyOccupancyStat
{
    public string Month { get; set; } = string.Empty;
    public double Percentage { get; set; }
}

public class HallResponse { public int Id { get; set; } public string Name { get; set; } = ""; public int Capacity { get; set; } public string Location { get; set; } = ""; public string Facilities { get; set; } = ""; }
public class BookingRequest { public int HallId { get; set; } public DateTime StartTime { get; set; } public DateTime EndTime { get; set; } public string Purpose { get; set; } = ""; public string? AdditionalInfo { get; set; } }
public class BookingResponse 
{ 
    public int Id { get; set; } 
    public string HallName { get; set; } = ""; 
    public string UserFullName { get; set; } = ""; 
    public DateTime StartTime { get; set; } 
    public DateTime EndTime { get; set; } 
    public string Purpose { get; set; } = ""; 
    public string? AdditionalInfo { get; set; }
    public int Status { get; set; } // 0: Pending, 1: Approved...
    public DateTime CreatedAt { get; set; }
}

public class UserDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = "";
    
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = "";
    
    [JsonPropertyName("role")]
    public string Role { get; set; } = "";
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}

public class CreateUserRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = "";
    
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = "";
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = "";
    
    [JsonPropertyName("role")]
    public string Role { get; set; } = "Officer";
}

public class UpdateUserRequest
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = "";
    
    [JsonPropertyName("role")]
    public string Role { get; set; } = "";
    
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
