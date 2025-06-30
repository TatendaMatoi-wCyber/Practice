using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeatherForecast.Pages
{
    public class SysPageModel:PageModel
    {
        OpenWeather? _openWeather;
        public OpenWeather OpenWeather
        {
            get
            {
                if (_openWeather == null)
                {
                    _openWeather = Request.HttpContext.RequestServices.GetRequiredService<OpenWeather>();
                }
                return _openWeather; 
            }
        }
    }
}
