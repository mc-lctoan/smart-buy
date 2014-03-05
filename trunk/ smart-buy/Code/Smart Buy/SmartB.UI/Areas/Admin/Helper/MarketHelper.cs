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
                    double lat1 = Double.Parse(markets[i].Latitude);
                    double lng1 = Double.Parse(markets[i].Longitude);

                    for (int j = i + 1; j < markets.Count; j++)
                    {
                        double lat2 = Double.Parse(markets[j].Latitude);
                        double lng2 = Double.Parse(markets[j].Longitude);

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