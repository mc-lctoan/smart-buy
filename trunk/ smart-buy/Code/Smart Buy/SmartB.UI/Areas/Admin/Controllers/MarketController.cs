using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models.EntityFramework;
using PagedList;

namespace SmartB.UI.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "staff")]
    public class MarketController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();
        private const int PageSize = 10;

        public ActionResult Index(int page = 1)
        {
            var markets = context.Markets
                .OrderBy(x => x.Name)
                .Where(x => x.IsActive && x.Address != null)
                .ToPagedList(page, PageSize);
            return View(markets);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Market model)
        {
            // Not exist
            var market = context.Markets
                .FirstOrDefault(x => x.Latitude == model.Latitude && x.Longitude == model.Longitude);
            if (market == null)
            {
                model.IsActive = true;
                context.Markets.Add(model);
                context.SaveChanges();
                TempData["create"] = "Success";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Chợ này đã có trong hệ thống.");

            return View();
        }

        public ActionResult Edit(int id)
        {
            var market = context.Markets.FirstOrDefault(x => x.Id == id);
            return View(market);
        }

        [HttpPost]
        public RedirectToRouteResult Edit(Market model)
        {
            var market = context.Markets.FirstOrDefault(x => x.Id == model.Id);
            string message;
            if (market != null)
            {
                market.Name = model.Name;
                market.Address = model.Address;
                market.Latitude = model.Latitude;
                market.Longitude = model.Longitude;
                context.SaveChanges();
                message = "Success";
            }
            else
            {
                message = "Failed";
            }
            TempData["edit"] = message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToRouteResult Delete(int[] ids)
        {
            foreach (var id in ids)
            {
                var market = context.Markets.FirstOrDefault(x => x.Id == id);
                if (market != null)
                {
                    market.IsActive = false;
                }
            }
            context.SaveChanges();
            TempData["delete"] = "Done";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
