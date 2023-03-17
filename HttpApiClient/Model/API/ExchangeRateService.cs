using HttpApiClient.Model.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace HttpApiClient.Model.API
{
    public class ExchangeRateService
    {
        private static readonly string BASE_URL = "https://openexchangerates.org/api/";
        private static readonly string APP_ID = "e77f99c02f404d34a3631b67223d85e5";

        public static async Task<ServiceResponse> GetExchangeRateConversion()
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            using (var httpClient = new HttpClient())
            {
                HttpRequestMessage request;
                HttpResponseMessage response;
                try
                {
                    /*
                     * Parameter "prettyprint" sets the json string format to plain text. 
                     */
                    string url = string.Format("{0}latest.json?app_id={1}&prettyprint=false", BASE_URL, APP_ID);
                    request = new HttpRequestMessage(HttpMethod.Get, url);
                    response = await httpClient.SendAsync(request);

                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            ExchangeRate exchangeRate;
                            String jsonstring = await response.Content.ReadAsStringAsync();

                            exchangeRate = JsonConvert.DeserializeObject<ExchangeRate>(jsonstring);


                            if (exchangeRate != null && exchangeRate.Disclaimer != null && exchangeRate.Rates != null)
                            {
                                serviceResponse.Error = false;
                                serviceResponse.Message = "OK";
                                serviceResponse.ExchangeRate = exchangeRate;
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
