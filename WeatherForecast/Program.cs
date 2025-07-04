namespace WeatherForecast
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var weatherApiKey = builder.Configuration.GetValue<string>("OpenWeather:ApiKey");
            if (weatherApiKey != null)
            {
                var openWeather = new OpenWeather(weatherApiKey);
                builder.Services.AddSingleton(openWeather);
            }
            else
            {
                throw new Exception("Open weather key not found");
            }
            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
