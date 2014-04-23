using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Models
{
    public class SuggestRouteModel
    {
        public string ProductName { get; set; }
        public string MarketName { get; set; }
        public int Price { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class OtherRoute
    {
        public string StartAddress { get; set; }
        public Coordinate StartPoint { get; set; }
        public string EndAddress { get; set; }
        public Coordinate EndPoint { get; set; }
        public string NearbyMarkets { get; set; }
        public string Waypoints { get; set; }
    }
}