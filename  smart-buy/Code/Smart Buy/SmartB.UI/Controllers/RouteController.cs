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
    public class RouteController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();

        public ActionResult DefineRoute()
        {
            var markets = context.Markets.Where(x => x.IsActive).ToList();
            return View(markets);
        }

        [HttpPost]
        public ActionResult DefineRoute(DefineRouteModel model)
        {
            if (model.Route == "")
            {
                ModelState.AddModelError("", "Phải chọn đường đi!");
            }

            if (ModelState.IsValid)
            {
                var user = context.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                if (user != null)
                {
                    user.DefinedRoute = model.Route;
                    user.MarketId = model.MarketId;
                    context.SaveChanges();
                    TempData["DefineRoute"] = "Success";
                    return RedirectToAction("DefineRoute");
                }
                ModelState.AddModelError("", "Username không tồn tại!");
            }

            // Something's wrong, redisplay the form
            var markets = context.Markets.Where(x => x.IsActive).ToList();
            return View(markets);
        }

        [Authorize]
        public ActionResult SuggestRoute(Cart cart)
        {
            var model = new List<SuggestRouteModel>();

            var cartProducts = cart.Lines.Select(x => x.Product.Product).ToList();

            var user = context.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
            if (user != null)
            {
                // TODO: haven't defined? Redirect to define route, show error
                // Get route
                ViewBag.Route = user.DefinedRoute;

                // Get nearby markets id
                string[] ids = user.MarketId.Split(',');

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

                var route = new SuggestRouteHelper(products, markets);
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
