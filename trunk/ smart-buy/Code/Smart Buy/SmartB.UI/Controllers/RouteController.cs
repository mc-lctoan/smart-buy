using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Helper;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Controllers
{
    [Authorize]
    public class RouteController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();

        public ActionResult DefineRoute()
        {
            var markets = context.Markets.Where(x => x.IsActive).ToList();
            return View(markets);
        }

        [HttpPost]
        public ActionResult DefineRoute(AccountDetailModel model)
        {
            if (ModelState.IsValid)
            {
                var user = context.Users
                    .Include(x => x.Profile)
                    .FirstOrDefault(x => x.Username == User.Identity.Name);
                if (user != null)
                {
                    var profile = new Profile
                                      {
                                          FirstStartAddress = model.FirstStartAddress,
                                          FirstEndAddress = model.FirstEndAddress,
                                          FirstStartDistance = null,
                                          FirstEndDistance = null,
                                          FirstRouteName = model.FirstRouteName,
                                          FirstRoute = model.FirstRoute,
                                          FirstMarkets = model.FirstMarkets,

                                          SecondStartAddress = model.SecondStartAddress,
                                          SecondEndAddress = model.SecondEndAddress,
                                          SecondStartDistance = null,
                                          SecondEndDistance = null,
                                          SecondRouteName = model.SecondRouteName,
                                          SecondRoute = model.SecondRoute,
                                          SecondMarkets = model.SecondMarkets,

                                          ThirdStartAddress = model.ThirdStartAddress,
                                          ThirdEndAddress = model.ThirdEndAddress,
                                          ThirdStartDistance = null,
                                          ThirdEndDistance = null,
                                          ThirdRouteName = model.ThirdRouteName,
                                          ThirdRoute = model.ThirdRoute,
                                          ThirdMarkets = model.ThirdMarkets
                                      };
                    user.Profile = profile;

                    context.SaveChanges();
                    TempData["DefineRoute"] = "Success";
                    return RedirectToAction("AccountDetails", "Account");
                }
                ModelState.AddModelError("", "Username không tồn tại!");
            }

            // Something's wrong, redisplay the form
            TempData["DefineRoute"] = "Fail";
            return RedirectToAction("AccountDetails", "Account");
        }

        public ActionResult SuggestRoute()
        {
            var model = new List<SuggestRouteModel>();
            var routes = new List<SelectListItem>();
            var markets = context.Markets
                .Where(x => x.IsActive && x.Latitude != null && x.Longitude != null)
                .ToList();
            ViewBag.Markets = markets;

            var user = context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(x => x.Username == User.Identity.Name);
            if (user != null)
            {
                if (user.Profile != null)
                {
                    if (user.Profile.FirstRoute != null)
                    {
                        var item = new SelectListItem
                                       {
                                           Text = user.Profile.FirstRouteName,
                                           Value = user.Profile.FirstRouteName
                                       };
                        routes.Add(item);
                    }
                    if (user.Profile.SecondRoute != null)
                    {
                        var item = new SelectListItem
                                       {
                                           Text = user.Profile.SecondRouteName,
                                           Value = user.Profile.SecondRouteName
                                       };
                        routes.Add(item);
                    }
                    if (user.Profile.ThirdRoute != null)
                    {
                        var item = new SelectListItem
                                       {
                                           Text = user.Profile.ThirdRouteName,
                                           Value = user.Profile.ThirdRouteName
                                       };
                        routes.Add(item);
                    }
                }
                var other = new SelectListItem
                                {
                                    Text = "Đường đi khác",
                                    Value = "Other"
                                };
                routes.Add(other);
                ViewBag.Routes = routes;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SuggestRoute(Cart cart, string Routes, OtherRoute other)
        {
            var allMarkets = context.Markets
                .Where(x => x.IsActive && x.Latitude != null && x.Longitude != null)
                .ToList();
            ViewBag.Markets = allMarkets;

            var model = new List<SuggestRouteModel>();
            var routes = new List<SelectListItem>();
            ViewBag.Other = other;

            var cartProducts = cart.Lines.Select(x => x.Product.Product).ToList();

            var user = context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(x => x.Username == User.Identity.Name);
            if (user != null)
            {
                // Construct the dropdownlist
                if (user.Profile.FirstRoute != null)
                {
                    var item = new SelectListItem
                    {
                        Text = user.Profile.FirstRouteName,
                        Value = user.Profile.FirstRouteName
                    };
                    routes.Add(item);
                }
                if (user.Profile.SecondRoute != null)
                {
                    var item = new SelectListItem
                    {
                        Text = user.Profile.SecondRouteName,
                        Value = user.Profile.SecondRouteName
                    };
                    routes.Add(item);
                }
                if (user.Profile.ThirdRoute != null)
                {
                    var item = new SelectListItem
                    {
                        Text = user.Profile.ThirdRouteName,
                        Value = user.Profile.ThirdRouteName
                    };
                    routes.Add(item);
                }
                var otherOption = new SelectListItem
                                {
                                    Text = "Đường đi khác",
                                    Value = "Other"
                                };
                routes.Add(otherOption);
                ViewBag.Routes = routes;

                if (other.Waypoints == null)
                {
                    return View(model);
                }

                string chosenRoute = "";
                string[] ids = null;
                var distanceA = new List<double>();
                var distanceB = new List<double>();

                // Other route
                if (Routes == "Other")
                {
                    // Get route
                    ViewBag.Route = other.Waypoints;
                    string[] idsOther = new string[0];

                    if (other.NearbyMarkets != null)
                    {
                        idsOther = other.NearbyMarkets.Split(',');
                    }

                    List<double> distanceAOther = CalculateDistanceHelper.DistanceToAllMarket(other.Waypoints, idsOther, "start");
                    List<double> distanceBOther = CalculateDistanceHelper.DistanceToAllMarket(other.Waypoints, idsOther, "end");

                    // Construct a market list
                    var marketsOther = new List<Market>();
                    foreach (string id in idsOther)
                    {
                        int tmp = Int32.Parse(id);
                        var market = context.Markets.FirstOrDefault(x => x.Id == tmp);
                        if (market != null)
                        {
                            marketsOther.Add(market);
                        }
                    }

                    // Construct a product list
                    var productsOther = new List<Product>();
                    foreach (var product in cartProducts)
                    {
                        var tmp = context.Products
                            .Include(x => x.SellProducts)
                            .Include(x => x.ProductAttributes)
                            .FirstOrDefault(x => x.Id == product.Id);
                        if (tmp != null)
                        {
                            productsOther.Add(tmp);
                        }
                    }

                    var routeOther = new SuggestRouteHelper(productsOther, marketsOther, distanceAOther, distanceBOther);
                    model = routeOther.Suggest();
                    return View(model);
                }

                if (user.Profile.FirstRouteName == Routes)
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
                else if (user.Profile.SecondRouteName == Routes)
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
                else if (user.Profile.ThirdRouteName == Routes)
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
                
                // Get route
                ViewBag.Route = chosenRoute;

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
                foreach (var product in cartProducts)
                {
                    var tmp = context.Products
                        .Include(x => x.SellProducts)
                        .Include(x => x.ProductAttributes)
                        .FirstOrDefault(x => x.Id == product.Id);
                    if (tmp != null)
                    {
                        products.Add(tmp);
                    }
                }

                var route = new SuggestRouteHelper(products, markets, distanceA, distanceB);
                model = route.Suggest();
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
