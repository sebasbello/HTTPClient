using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForecastClient.Model.Object;

namespace ForecastClient.Model.API
{
    public class ServiceResponse
    {
        public bool Error { get; set; }
        public String Message { get; set; }
        public Current Current { get; set; }
    }
}
