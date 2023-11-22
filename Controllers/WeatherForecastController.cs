using Microsoft.AspNetCore.Mvc;

namespace SerilogCorealtionIdLoggingDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        ICorrelationIdGenerator _correlationIdGenerator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICorrelationIdGenerator correlationIdGenerator)
        {
            _logger = logger;
            _correlationIdGenerator = correlationIdGenerator;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
           _logger.LogInformation("GetWeatherForecast Called");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
