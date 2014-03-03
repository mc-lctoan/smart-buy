using System;
using System.Collections.Generic;
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
                // TODO: Fix username for testing. Must use current login username later.
                var user = context.Users.FirstOrDefault(x => x.Username == "Sergey Pimenov");
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

        public ActionResult SuggestRoute(Cart cart)
        {
            var products = cart.Lines.Select(x => x.Product.Product);
            List<int> listProductId = products.Select(x => x.Id).ToList();

            var user = context.Users.FirstOrDefault(x => x.Username == "Sergey Pimenov");
            if (user != null)
            {
                string[] ids = user.MarketId.Split(',');
                var marketIds = new List<int>();
                foreach (string id in ids)
                {
                    marketIds.Add(Int32.Parse(id));
                }
                var route = new SuggestRouteHelper();
                var result = route.Suggest(listProductId, marketIds);
                TempData["RouteResult"] = result;
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
