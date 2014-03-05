using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Areas.Admin.Helper;
using SmartB.UI.Models.EntityFramework;
using PagedList;

namespace SmartB.UI.Areas.Admin.Controllers
{
    public class MarketController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();
        private const int PageSize = 10;

        public ActionResult Index(int page = 1)
        {
            var markets = context.Markets
                .OrderBy(x => x.Name)
                .Where(x => x.IsActive)
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

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
