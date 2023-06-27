using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyClient.Model.Object
{
    public class ExchangeRate
    {
        public String Disclaimer { get; set; }
        public String License { get; set; }
        public long Timestamp { get; set; }
        public String Base { get; set; }
        public Dictionary<String, Double> Rates { get; set; }
    }
}
