using Moq;
using HelloYamlCore.Services;
using Newtonsoft.Json;
using System.Net;

namespace HelloYamlCore.Api.Tests
{
    public class WeatherForecastTests
    {
        private readonly TestMoqPOCApplication _testMoqPOCApplication;
        private readonly MockServices _mockServices;
        public readonly HttpClient _client;

        public WeatherForecastTests()
        {
            _mockServices = new MockServices();
            //instantiating TestMoqPOCApplication
            _testMoqPOCApplication = new TestMoqPOCApplication(_mockServices);
            //creating client for api call
            _client = _testMoqPOCApplication.CreateClient();
        }

        [Fact]
        public async void GetWeatherForecastTest()
        {
            //mocking the business service's GetWeatherForecast() method to return result as below  
            var expResult = new WeatherForecast[]
            {
                new WeatherForecast(DateTime.Now, 26, "Bengaluru")
            };
            //mocking the business service's GetWeatherForecast()
            _mockServices.WeatherForecastServiceMock.Setup(m => m.GetWeatherForecast()).ReturnsAsync(expResult);
            //calling the api
            var response = await _client.GetAsync("/WeatherForecast");
            string jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<WeatherForecast[]>(jsonString);
            //testing the response
            Assert.Equal(response.StatusCode, HttpStatusCode.OK);
            Assert.Equal(expResult, result);
        }
    }
}
