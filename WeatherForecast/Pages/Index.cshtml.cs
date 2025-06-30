using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WeatherForecast.Models;
using WeatherForecast;
using System.Text.RegularExpressions;

namespace WeatherForecast.Pages
{
    public class IndexModel : SysPageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public WeatherResponse? WeatherResponse { get; set; }
        public async Task OnGet(string? city)
        {

            if (!string.IsNullOrWhiteSpace(city))
            {
                city = city.Trim();
                 city = Regex.Replace(city, "<,*.>?/&%#@!",string.Empty);
                WeatherResponse = await OpenWeather.GetCurrentWeather(city);
            }
            else
            {
                WeatherResponse = await OpenWeather.GetCurrentWeather("Harare");
            }
        }
        
    }
}
