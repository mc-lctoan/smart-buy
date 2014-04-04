using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Models
{
    public class RouteModel
    {
        public Coordinate Start { get; set; }
        public Coordinate End { get; set; }
        public int[] Waypoints { get; set; }
    }

    public class Coordinate
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}