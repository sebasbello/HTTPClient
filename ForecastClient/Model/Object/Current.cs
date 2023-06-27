using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using ForecastClient.Model.Object.Field;

namespace ForecastClient.Model.Object
{
    public class Current
    {
        public Coord Coord { get; set; }
        public Weather[] Weather { get; set; }
        public Main Main { get; set; }
        public Sys Sys { get; set; }
        public String Id { get; set; }
        public String Name { get; set; }
    }
}
