using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartB.UI.Helper;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Areas.Admin.Helper
{
    public static class MarketHelper
    {
        public static void CalculateDistance()
        {
            var math = new MathHelper();

            using (var context = new SmartBuyEntities())
            {
                var markets = context.Markets.Where(x => x.IsActive).ToList();
                for (int i = 0; i < markets.Count - 1; i++)
                {
                    double lat1 = markets[i].Latitude.Value;
                    double lng1 = markets[i].Longitude.Value;

                    for (int j = i + 1; j < markets.Count; j++)
                    {
                        if (markets[j].Latitude == null || markets[j].Longitude == null)
                        {
                            continue;
                        }
                        double lat2 = markets[j].Latitude.Value;
                        double lng2 = markets[j].Longitude.Value;

                        double distance = math.CalculateDistance(lat1, lng1, lat2, lng2);

                        int fromId = markets[i].Id;
                        int toId = markets[j].Id;

                        var mDis = context.MarketDistances
                            .FirstOrDefault(x => x.FromMarket == fromId && x.ToMarket == toId);
                        if (mDis == null)
                        {
                            var dis = new MarketDistance
                                          {
                                              FromMarket = markets[i].Id,
                                              ToMarket = markets[j].Id,
                                              Distance = distance
                                          };
                            context.MarketDistances.Add(dis);
                        }
                    }
                }
                context.SaveChanges();
            }
        }
    }
}