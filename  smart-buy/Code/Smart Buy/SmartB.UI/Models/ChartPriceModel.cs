using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Models
{
    public class ChartPriceModel
    {
        public long Time { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }
}