using System.Net.Http.Json;

namespace CarGoCarWebApp.Services;

public class AuthService
{
    private readonly HttpClient _http;
    
    public int? UserId { get; private set; }
    public string? UserRole { get; private set; }
    public string? Token { get; private set; }
    public bool IsAuthenticated => UserId.HasValue;
    public bool IsAdmin => UserRole == "Admin";
    public bool IsDriver => UserRole == "Driver";
    public bool IsPassenger => UserRole == "Passenger";

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient http) => _http = http;

    public async Task<(bool success, string? error)> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", new { email, password });
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            UserId = result?.UserId;
            UserRole = result?.Role;
            Token = result?.Token;
            OnAuthStateChanged?.Invoke();
            return (true, null);
        }
        
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        return (false, error?.Error ?? "Login failed");
    }

    public async Task<(bool success, string? error)> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", new { email, password, firstName, lastName });
        
        if (response.IsSuccessStatusCode)
            return (true, null);
        
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        return (false, error?.Error ?? "Registration failed");
    }

    public void Logout()
    {
        UserId = null;
        UserRole = null;
        Token = null;
        OnAuthStateChanged?.Invoke();
    }

    private class LoginResponse
    {
        public string? Token { get; set; }
        public int UserId { get; set; }
        public string? Role { get; set; }
    }

    private class ErrorResponse
    {
        public string? Error { get; set; }
    }
}

