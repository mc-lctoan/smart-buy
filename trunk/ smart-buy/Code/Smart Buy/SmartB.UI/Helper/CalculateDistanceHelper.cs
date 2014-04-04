using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Helper
{
    public static class CalculateDistanceHelper
    {
        public static void CalculateDistance()
        {
            var model = DistanceConfigHelper.GetData();
            if (model.Status == (int)DistanceStatus.Finish)
            {
                return;
            }
            int i = model.From;
            int j = model.To;
            var math = new MathHelper();

            using (var context = new SmartBuyEntities())
            {
                var markets = context.Markets
                    .Where(x => x.IsActive && x.Latitude != null && x.Longitude != null)
                    .ToList();
                while (true)
                {
                    var lat1 = markets[i].Latitude.Value;
                    var lng1 = markets[i].Longitude.Value;
                    var lat2 = markets[j].Latitude.Value;
                    var lng2 = markets[j].Longitude.Value;
                    var distance = math.TravelDistance(lat1, lng1, lat2, lng2);

                    // OK
                    if (distance != -1)
                    {
                        var m1 = markets[i];
                        var m2 = markets[j];
                        var tmp = context.MarketDistances
                            .FirstOrDefault(x => x.FromMarket == m1.Id && x.ToMarket == m2.Id);
                        if (tmp != null)
                        {
                            tmp.Distance = distance;
                        }
                        else
                        {
                            var mDistance = new MarketDistance
                                                {
                                                    FromMarket = markets[i].Id,
                                                    ToMarket = markets[j].Id,
                                                    Distance = distance
                                                };
                            context.MarketDistances.Add(mDistance);
                        }

                        j++;
                        if (j == markets.Count)
                        {
                            i++;

                            // Finish
                            if (i == markets.Count - 1)
                            {
                                DistanceConfigHelper.Finish();
                                context.SaveChanges();
                                return;
                            }
                            j = i + 1;
                        }
                    }
                    else
                    {
                        // Something's wrong, out of quota
                        var savePoint = new DistanceConfigModel
                                            {
                                                From = i,
                                                To = j,
                                                Status = (int) DistanceStatus.Going
                                            };

                        // Save
                        DistanceConfigHelper.SetData(savePoint);
                        context.SaveChanges();
                        return;
                    }
                }
            }
        }

        public static List<double> DistanceToAllMarket(string route, string[] ids, string type)
        {
            var math = new MathHelper();
            var result = new List<double>();
            var markets = new List<Market>();
            var model = JsonConvert.DeserializeObject<RouteModel>(route);

            using (var context = new SmartBuyEntities())
            {
                // Construct a market list
                foreach (string id in ids)
                {
                    int tmp = Int32.Parse(id);
                    var market = context.Markets.FirstOrDefault(x => x.Id == tmp);
                    if (market != null)
                    {
                        markets.Add(market);
                    }
                }
            }

            Coordinate point = new Coordinate();
            switch (type)
            {
                case "start":
                    point = model.Start;
                    break;
                case "end":
                    point = model.End;
                    break;
            }

            foreach (Market market in markets)
            {
                double distance = math.TravelDistance(point.Lat, point.Lng, market.Latitude.Value, market.Longitude.Value);
                result.Add(distance);
            }

            return result;
        }
    }
}