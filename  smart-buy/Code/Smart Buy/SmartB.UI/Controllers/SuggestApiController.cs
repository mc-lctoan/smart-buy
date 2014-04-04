using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartB.UI.Helper;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;
using SmartB.UI.Models.MobileModel;

namespace SmartB.UI.Controllers
{
    public class SuggestApiController : ApiController
    {
        private SmartBuyEntities context = new SmartBuyEntities();

        [AcceptVerbs("POST")]
        public List<SuggestRouteModel> SuggestRoute(SuggestMobileModel model)
        {
            var result = new List<SuggestRouteModel>();
            var user = context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(x => x.Username == model.Username);
            if (user != null)
            {
                string[] ids = null;
                string chosenRoute;
                var distanceA = new List<double>();
                var distanceB = new List<double>();

                if (user.Profile.FirstRouteName == model.RouteName)
                {
                    chosenRoute = user.Profile.FirstRoute;
                    ids = user.Profile.FirstMarkets.Split(',');
                    if (user.Profile.FirstStartDistance != null)
                    {
                        string[] distances = user.Profile.FirstStartDistance.Split(',');
                        foreach (string s in distances)
                        {
                            double tmp = Double.Parse(s);
                            distanceA.Add(tmp);
                        }
                    }
                    else
                    {
                        distanceA = CalculateDistanceHelper.DistanceToAllMarket(chosenRoute, ids, "start");
                        string tmp = "";
                        foreach (double d in distanceA)
                        {
                            tmp += d + ",";
                        }
                        tmp = tmp.Remove(tmp.Length - 1);
                        user.Profile.FirstStartDistance = tmp;
                    }
                    if (user.Profile.FirstEndDistance != null)
                    {
                        string[] distances = user.Profile.FirstEndDistance.Split(',');
                        foreach (string s in distances)
                        {
                            double tmp = Double.Parse(s);
                            distanceB.Add(tmp);
                        }
                    }
                    else
                    {
                        distanceB = CalculateDistanceHelper.DistanceToAllMarket(chosenRoute, ids, "end");
                        string tmp = "";
                        foreach (double d in distanceB)
                        {
                            tmp += d + ",";
                        }
                        tmp = tmp.Remove(tmp.Length - 1);
                        user.Profile.FirstEndDistance = tmp;
                    }
                }
                else if (user.Profile.SecondRouteName == model.RouteName)
                {
                    chosenRoute = user.Profile.SecondRoute;
                    ids = user.Profile.SecondMarkets.Split(',');
                    if (user.Profile.SecondStartDistance != null)
                    {
                        string[] distances = user.Profile.SecondStartDistance.Split(',');
                        foreach (string s in distances)
                        {
                            double tmp = Double.Parse(s);
                            distanceA.Add(tmp);
                        }
                    }
                    else
                    {
                        distanceA = CalculateDistanceHelper.DistanceToAllMarket(chosenRoute, ids, "start");
                        string tmp = "";
                        foreach (double d in distanceA)
                        {
                            tmp += d + ",";
                        }
                        tmp = tmp.Remove(tmp.Length - 1);
                        user.Profile.SecondStartDistance = tmp;
                    }
                    if (user.Profile.SecondEndDistance != null)
                    {
                        string[] distances = user.Profile.SecondEndDistance.Split(',');
                        foreach (string s in distances)
                        {
                            double tmp = Double.Parse(s);
                            distanceB.Add(tmp);
                        }
                    }
                    else
                    {
                        distanceB = CalculateDistanceHelper.DistanceToAllMarket(chosenRoute, ids, "end");
                        string tmp = "";
                        foreach (double d in distanceB)
                        {
                            tmp += d + ",";
                        }
                        tmp = tmp.Remove(tmp.Length - 1);
                        user.Profile.SecondEndDistance = tmp;
                    }
                }
                else if (user.Profile.ThirdRouteName == model.RouteName)
                {
                    chosenRoute = user.Profile.ThirdRoute;
                    ids = user.Profile.ThirdMarkets.Split(',');
                    if (user.Profile.ThirdStartDistance != null)
                    {
                        string[] distances = user.Profile.ThirdStartDistance.Split(',');
                        foreach (string s in distances)
                        {
                            double tmp = Double.Parse(s);
                            distanceA.Add(tmp);
                        }
                    }
                    else
                    {
                        distanceA = CalculateDistanceHelper.DistanceToAllMarket(chosenRoute, ids, "start");
                        string tmp = "";
                        foreach (double d in distanceA)
                        {
                            tmp += d + ",";
                        }
                        tmp = tmp.Remove(tmp.Length - 1);
                        user.Profile.ThirdStartDistance = tmp;
                    }
                    if (user.Profile.ThirdEndDistance != null)
                    {
                        string[] distances = user.Profile.ThirdEndDistance.Split(',');
                        foreach (string s in distances)
                        {
                            double tmp = Double.Parse(s);
                            distanceB.Add(tmp);
                        }
                    }
                    else
                    {
                        distanceB = CalculateDistanceHelper.DistanceToAllMarket(chosenRoute, ids, "end");
                        string tmp = "";
                        foreach (double d in distanceB)
                        {
                            tmp += d + ",";
                        }
                        tmp = tmp.Remove(tmp.Length - 1);
                        user.Profile.ThirdEndDistance = tmp;
                    }
                }
                context.SaveChanges();

                // Construct a market list
                var markets = new List<Market>();
                foreach (string id in ids)
                {
                    int tmp = Int32.Parse(id);
                    var market = context.Markets.FirstOrDefault(x => x.Id == tmp);
                    if (market != null)
                    {
                        markets.Add(market);
                    }
                }

                // Construct a product list
                var products = new List<Product>();
                foreach (var id in model.ProductIds)
                {
                    var tmp = context.Products
                        .Include(x => x.SellProducts)
                        .Include(x => x.ProductAttributes)
                        .FirstOrDefault(x => x.Id == id);
                    if (tmp != null)
                    {
                        products.Add(tmp);
                    }
                }
                var helper = new SuggestRouteHelper(products, markets, distanceA, distanceB);

                result = helper.Suggest();
            }

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}