using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace CityWeather
{
    class Program
    {
        static void Main(string[] args)
        {
            var cityForecasts = new List<CityWeather>();
            string[] cities = { "Omaha", "Denver", "San Jose", "Phoenix", "Miami", "Boston" };
            
            //cityForecasts.Add(new CityWeather("Denver"));
            //cityForecasts.Add(new CityWeather("San Jose"));
            //cityForecasts.Add(new CityWeather("Phoenix"));
            //cityForecasts.Add(new CityWeather("Miami"));
            //cityForecasts.Add(new CityWeather("Boston"));

            foreach (var city in cities)
            {
                var apiCall = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=a6bf3dc24cf54af353aa915b839e088a&units=imperial";
                
                HttpClient client = new HttpClient();
                var responseTask = client.GetAsync(apiCall);
                responseTask.Wait();

                if (responseTask.IsCompleted)
                {
                    var result1 = responseTask.Result;
                    var messageTask = result1.Content.ReadAsStringAsync();
                    messageTask.Wait();

                    JObject WeatherForecastJO = JObject.Parse(messageTask.Result);

                    //var cityNameJO = WeatherForecastJO["name"];
                    var windSpeedJO = (int)WeatherForecastJO["wind"]["speed"];
                    var temperatureJO = (float)WeatherForecastJO["main"]["temp"];
                    var feelsLikeJO = (float)WeatherForecastJO["main"]["feels_like"];
                    var visibilityJO = (int)WeatherForecastJO["visibility"];
                    var humidityJO = (int)WeatherForecastJO["main"]["humidity"];

                    // Test JObject Output
                    // Console.WriteLine(WeatherForecastJO);


                    cityForecasts.Add(new CityWeather(city, windSpeedJO, temperatureJO, feelsLikeJO, visibilityJO, humidityJO));


                    //Console.WriteLine($"Here's the forecast for {cityNameJO}: " +
                    //    $"The temperature is {temperatureJO}F, but feels like {feelsLikeJO}F. " +
                    //    $"The windspeed is currently {windSpeedJO} mph, with visibility at {visibilityJO}. " +
                    //    $"Lastly, the humidity is at {humidityJO}%.");                    
                }

                else

                {                    
                    Console.WriteLine("Sorry. Something must have went wrong.");
                }
            }
            foreach (var city in cityForecasts)
            {
                Console.WriteLine(city);
            }
            Console.ReadLine();
        }

        class CityWeather
        {
            private string _cityName;
            private int _windSpeed;
            private float _temperatureF;
            private float _feelsLike;
            private int _visibility;
            private int _humidity;

            public CityWeather(

            string cityName,
            int windSpeed,
            float temperatureF,
            float feelsLike,
            int visibility,
            int humidity)

            {
                _cityName = cityName;
                _windSpeed = windSpeed;
                _temperatureF = temperatureF;
                _feelsLike = feelsLike;
                _visibility = visibility;
                _humidity = humidity;

            }

            public override string ToString()
            {

                return $"Here's the forecast for {_cityName}: The temperature is {_temperatureF}F, but feels like {_feelsLike}F. " +
                    $"The windspeed is currently {_windSpeed} mph, with visibility at {_visibility}. " +
                    $"Lastly, the humidity is at {_humidity}%.";
            
            }
            public string getCityName()

            {
                return _cityName;
            }

            public int getWindSpeed()
            {
                return _windSpeed;
            }

            //public float getTemperatureF()

            //{
            //    return _temperatureF;
            //}

            //public int getVisibility()
            //{
            //    return _visibility;
            //}

            //public float getFeelsLike()
            //{
            //    return _feelsLike;
            //}



        }
    }
}
