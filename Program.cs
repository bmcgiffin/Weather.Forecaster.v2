using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


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

            NewRelic.PostToNewRelic(cityForecasts);
        }

        public class CityWeather
        {
            public string eventType { get; set; }
            public string cityName { get; set; }
            public int windSpeed { get; set; }
            public float temperatureF { get; set; }
            public float feelsLike { get; set; }
            public int visibility { get; set; }
            public int humidity { get; set; }


            public CityWeather(
            
            string _cityName,
            int _windSpeed,
            float _temperatureF,
            float _feelsLike,
            int _visibility,
            int _humidity)

            {
                eventType = "bm_test2";
                cityName = _cityName;
                windSpeed = _windSpeed;
                temperatureF = _temperatureF;
                feelsLike = _feelsLike;
                visibility = _visibility;
                humidity = _humidity;

            }

            public override string ToString()
            {

                return $"Here's the forecast for {cityName}: The temperature is {temperatureF}F, but feels like {feelsLike}F. " +
                    $"The windspeed is currently {windSpeed} mph, with visibility at {visibility}. " +
                    $"Lastly, the humidity is at {humidity}%.";

            }
            public string getCityName()

            {
                return cityName;
            }

            public int getWindSpeed()
            {
                return windSpeed;
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

        public static class NewRelic
        {
            public static void PostToNewRelic(List<CityWeather> results)
            {
                //new Relic info 
                string myAccountID = "X";
                string myInsertKey = "X";
                string newRelicUrl = "https://insights-collector.newrelic.com/v1/accounts/" + myAccountID + "/events";
                string jsonString = JsonSerializer.Serialize(results);
                Console.WriteLine(jsonString); 



                var httpWebRequest = (HttpWebRequest)WebRequest.Create(newRelicUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("X-Insert-Key", myInsertKey);
                httpWebRequest.Headers.Add("Accept-Encoding", "gzip");
                httpWebRequest.Method = "POST";


                

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonString);
                }



                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }



                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Data sent to new relic");
                }
                else
                {
                    Console.Write("Error received when publishing to New Relic.\n{0}", httpResponse.StatusCode);
                }



            }
        }
    }
}

