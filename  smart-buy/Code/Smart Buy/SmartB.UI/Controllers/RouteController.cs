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
                                          FirstRouteName = model.FirstRouteName,
                                          FirstRoute = model.FirstRoute,
                                          FirstMarkets = model.FirstMarkets,
                                          SecondRouteName = model.SecondRouteName,
                                          SecondRoute = model.SecondRoute,
                                          SecondMarkets = model.SecondMarkets,
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
            var user = context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(x => x.Username == User.Identity.Name);
            if (user != null)
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
                ViewBag.Routes = routes;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SuggestRoute(Cart cart, string Routes)
        {
            var model = new List<SuggestRouteModel>();
            var routes = new List<SelectListItem>();

            var cartProducts = cart.Lines.Select(x => x.Product.Product).ToList();

            var user = context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(x => x.Username == User.Identity.Name);
            if (user != null)
            {
                string chosenRoute = "";
                string[] ids = null;

                if (user.Profile.FirstRouteName == Routes)
                {
                    chosenRoute = user.Profile.FirstRoute;
                    ids = user.Profile.FirstMarkets.Split(',');
                } 
                else if (user.Profile.SecondRouteName == Routes)
                {
                    chosenRoute = user.Profile.SecondRoute;
                    ids = user.Profile.SecondMarkets.Split(',');
                }
                else if (user.Profile.ThirdRouteName == Routes)
                {
                    chosenRoute = user.Profile.ThirdRoute;
                    ids = user.Profile.ThirdMarkets.Split(',');
                }
                
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

                var route = new SuggestRouteHelper(products, markets, HttpContext.Application["ConfigPath"].ToString());
                model = route.Suggest();

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
                ViewBag.Routes = routes;
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
