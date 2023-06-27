using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastClient.Model.Object.Field
{
    public class Weather
    {
        public String Id { get; set; }
        public String Main { get; set; }
        public String Description { get; set; }
        public String Icon { get; set; }

    }
}
