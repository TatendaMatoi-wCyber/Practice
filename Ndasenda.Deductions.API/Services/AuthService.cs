using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ndasenda.Deductions.API.Models;

namespace Ndasenda.Deductions.API.Services;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService()
    {
        _http = new HttpClient { BaseAddress = new Uri("https://sandbox.deductions.ndasenda.co.zw/") };
    }

    public async Task<AuthToken?> GetAccessTokenAsync(string username, string password)
    {
        var body = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        });

        var response = await _http.PostAsync("connect/token", body);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AuthToken>(json);
    }
}
