using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                    var lat1 = Double.Parse(markets[i].Latitude);
                    var lng1 = Double.Parse(markets[i].Longitude);
                    var lat2 = Double.Parse(markets[j].Latitude);
                    var lng2 = Double.Parse(markets[j].Longitude);
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
    }
}