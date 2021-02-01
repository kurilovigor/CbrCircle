using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CbrCircle
{
    /// <summary>
    /// Калсс настроек службы в файле appsettings.json
    /// </summary>
    public class RateServiceOptions : IRateServiceOptions
    {
        public string Currency { get; set; }
        public string Api { get; set; }
        public decimal Radius { get; set; }
    }

}
