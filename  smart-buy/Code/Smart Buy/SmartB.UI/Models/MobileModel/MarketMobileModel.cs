using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Models
{
    public class MarketMobileModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}