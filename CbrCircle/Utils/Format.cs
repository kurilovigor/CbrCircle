using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CbrCircle.Utils
{
    public static class Format
    {
        public static decimal ToDecimal(string value)
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
