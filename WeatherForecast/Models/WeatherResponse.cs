namespace WeatherForecast.Models
{
    public class WeatherResponse
    {
        public Coordinate Coord { get; set; } = null!;
        public string Base {  get; set; } = string.Empty;
        public Main Main { get; set; } = null!;
    }
}
