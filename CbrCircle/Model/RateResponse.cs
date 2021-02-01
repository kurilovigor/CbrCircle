using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CbrCircle.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class RateResponse
    {
        public decimal Rate { get; set; }
        public string Сurrency { get; set; }
        public DateTime Date { get; set; }
        public string Quater { get; set; }
        public decimal Radius { get; set; }
    }
}
