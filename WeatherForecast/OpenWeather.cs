using System.Text.Json;
using WeatherForecast.Models;

namespace WeatherForecast
{
    public class OpenWeather (string apiKey)
    {
        public string ApiKey { get; } = apiKey;
        public async Task<WeatherResponse?> GetCurrentWeather(string city)
        {
            
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={ApiKey}";
            var client = new HttpClient();
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                // Json Deserialization
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                };
                var weatherforecast = JsonSerializer.Deserialize<WeatherResponse>(content, options);
                return weatherforecast;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);


            }
            return null;

        }
    }
}
