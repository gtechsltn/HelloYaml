namespace HelloYamlCore.Services
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]> GetWeatherForecast();
    }
}
