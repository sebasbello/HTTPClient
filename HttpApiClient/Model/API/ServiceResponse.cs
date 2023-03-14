using HttpApiClient.Model.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpApiClient.Model.API
{
    public class ServiceResponse
    {
        public bool Error { get; set; }
        public String Message { get; set; }
        public ExchangeRate ExchangeRate { get; set; }
    }
}
