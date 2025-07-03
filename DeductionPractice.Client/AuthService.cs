using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;
using DeductionsPractice.Lib;

namespace DeductionPractice.Client
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public AuthService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://sandbox.deductions.ndasenda.co.zw")
            };
        }

        public async Task<AuthToken?> LoginAsync()
        {
            
            Console.WriteLine("Enter Username");
            var username = Console.ReadLine();

            Console.WriteLine("Enter Password");
            var password = Console.ReadLine();


            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            var response = await _http.PostAsync("/connect/token", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<AuthToken>(json, JsonSerializerService.Options);
            return token;
        }

        public async Task<AuthToken?> RefreshTokenAsync(string refreshToken)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            });

            var response = await _http.PostAsync("/connect/token", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<AuthToken>(json, JsonSerializerService.Options);
            return token;
        }
    }
}
