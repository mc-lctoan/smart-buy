using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Helper
{
    public class MathHelper
    {
        // Radius of the Earth in km
        private const int R = 6371;

        public double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            var dLat = DegreeToRad(lat2 - lat1);
            var dLon = DegreeToRad(lng2 - lng1);
            var a =
                Math.Sin(dLat/2)*Math.Sin(dLat/2) +
                Math.Cos(DegreeToRad(lat1))*Math.Cos(DegreeToRad(lat2))*
                Math.Sin(dLon/2)*Math.Sin(dLon/2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Distance in km
            var d = R * c;

            return d;
        }

        private double DegreeToRad(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}