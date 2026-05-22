using BookingSystem.Data;
using BookingSystem.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Services
builder.Services.AddScoped<BookingSystem.Api.Services.Interfaces.IAuthService, BookingSystem.Api.Services.AuthService>();
builder.Services.AddScoped<BookingSystem.Api.Services.Interfaces.IHallService, BookingSystem.Api.Services.HallService>();
builder.Services.AddScoped<BookingSystem.Api.Services.Interfaces.IBookingService, BookingSystem.Api.Services.BookingService>();
builder.Services.AddScoped<BookingSystem.Api.Services.Interfaces.IDashboardService, BookingSystem.Api.Services.DashboardService>();
builder.Services.AddScoped<BookingSystem.Api.Services.Interfaces.IUserService, BookingSystem.Api.Services.UserService>();
builder.Services.AddScoped<BookingSystem.Api.Services.Interfaces.IReportExportService, BookingSystem.Api.Services.ReportExportService>();

// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings.GetValue<string>("Secret") ?? "YourSuperSecretKey_ChangeThisInProduction_123456789";
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
