using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ForecastClient.Model.Object;

namespace ForecastClient.Model.API
{
    public class CurrentService
    {
        private static readonly string BASE_URL = "https://api.openweathermap.org/data/2.5/";
        private static readonly string APP_ID = "c7e3117f77b5a6b2a722cda37401c090";

        public static async Task<ServiceResponse> GetCurrentWeather(string location)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request;
                HttpResponseMessage response;

                try
                {
                    string url = string.Format("{0}weather?q={1}&appid={2}&lang=es&units=metric", BASE_URL, location, APP_ID);
                    request = new HttpRequestMessage(HttpMethod.Get, url);
                    response = await httpClient.SendAsync(request);

                    if (response != null)
                    {
                        if(response.IsSuccessStatusCode)
                        {
                            Current current;
                            String jsonString = await response.Content.ReadAsStringAsync();

                            current = JsonConvert.DeserializeObject<Current>(jsonString);

                            if (current != null)
                            {
                                serviceResponse.Error = false;
                                serviceResponse.Message = "OK";
                                serviceResponse.Current = current;
                            }
                            else
                            {
                                serviceResponse.Error = true;
                                serviceResponse.Message = "JSON serialization error...";
                            }
                        }
                        else
                        {
                            serviceResponse.Error = true;
                            serviceResponse.Message = String.Format("Error: {0} - {1}", (int)response.StatusCode, response.StatusCode);
                        }
                    }
                    else
                    {
                        serviceResponse.Error = true;
                        serviceResponse.Message = "Web service inaccesible...";
                    }
                }
                catch (Exception exception)
                {
                    serviceResponse.Error = true;
                    serviceResponse.Message = exception.Message;
                }
            }
            return serviceResponse;
        }
    }
}
