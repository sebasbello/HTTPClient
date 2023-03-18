using HttpApiClient.Model.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpApiClient.Model.API
{
    public class CurrencyService
    {
        private static readonly string BASE_URL = "https://openexchangerates.org/api/";
        private static readonly string APP_ID = "e77f99c02f404d34a3631b67223d85e5";

        public static async Task<ServiceResponse> GetCurrencies()
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            using (var httpClient = new HttpClient())
            {
                HttpRequestMessage request;
                HttpResponseMessage response;
                try
                {
                    string url = string.Format("{0}currencies.json?app_id={1}&prettyprint=false", BASE_URL, APP_ID);
                    request = new HttpRequestMessage(HttpMethod.Get, url);
                    response = await httpClient.SendAsync(request);

                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Currency currency = new Currency();
                            String jsonstring = await response.Content.ReadAsStringAsync();

                            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonstring);
                            currency.Currencies = values;

                            if (currency.Currencies != null)
                            {
                                serviceResponse.Error = false;
                                serviceResponse.Message = "OK";
                                serviceResponse.Currencies = currency.Currencies;
                            }
                            else
                            {
                                serviceResponse.Error = true;
                                serviceResponse.Message = "JSON serialization erorr....";
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
