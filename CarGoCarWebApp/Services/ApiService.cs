using System.Net.Http.Json;

namespace CarGoCarWebApp.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            return await _http.GetFromJsonAsync<T>($"/api/{endpoint}");
        }
        catch { return default; }
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        return await _http.PostAsJsonAsync($"/api/{endpoint}", data);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
    {
        return await _http.PutAsJsonAsync($"/api/{endpoint}", data);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        return await _http.DeleteAsync($"/api/{endpoint}");
    }
}

