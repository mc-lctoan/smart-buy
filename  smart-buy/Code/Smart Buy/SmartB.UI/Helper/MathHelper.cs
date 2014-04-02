using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

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

        public double TravelDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const string url = "https://maps.googleapis.com/maps/api/distancematrix/xml?";
            const string key = "AIzaSyAmQi6XuUHARe_gzLWFpKWzZSu34ZaWv1Q";
            const string sensor = "false";
            string origins = lat1 + "," + lng1;
            string destinations = lat2 + "," + lng2;

            //string fullPath = url + "key=" + key + "&origins=" + origins + "&destinations=" + destinations + "&sensor=" + sensor;
            string fullPath = url + "origins=" + origins + "&destinations=" + destinations + "&sensor=" + sensor;

            var client = new WebClient();
            Stream stream = client.OpenRead(fullPath);
            XDocument doc = XDocument.Load(stream);
            try
            {
                var root = doc.Element("DistanceMatrixResponse");
                var status = root.Element("status").Value;
                if (status != "OK")
                {
                    return -1;
                }

                var distance = root.Element("row")
                    .Element("element")
                    .Element("distance")
                    .Element("value");
                if (distance != null)
                {
                    double result;
                    bool flag = Double.TryParse(distance.Value, out result);
                    if (flag)
                    {
                        return result / 1000;
                    }
                }
            }
            catch (NullReferenceException)
            {
                return 0;
            }
            
            return 0;
        }
    }
}