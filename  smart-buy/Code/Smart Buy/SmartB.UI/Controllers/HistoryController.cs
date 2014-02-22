using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Models.EntityFramework;
using PagedList;
using System.Net;
using System.Data.Entity;
using SmartB.UI.Models;

namespace SmartB.UI.Controllers
{
    public class HistoryController : Controller
    {
        //
        // GET: /History/
        private SmartBuyEntities db = new SmartBuyEntities();
        public ActionResult BuyingHistory(int? page)
        {
            //page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            DateTime minusThirty = DateTime.Today.AddDays(-30);
            DateTime minusZero = DateTime.Today.AddDays(0);
            var history = from item in db.Histories
                          where item.BuyTime >= minusThirty && item.BuyTime <= minusZero && item.Username.Equals("Sergey Pimenov")
                          select item;
            history = history.OrderByDescending(item => item.BuyTime);
            return View(history.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult BuyingHistoryDetail(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var modelHistory = db.Histories.Include(h => h.HistoryDetails).Single(q => q.Id == id);
            var modelProduct = from p in db.ProductAttributes
                               group p by p.ProductId into grp
                               select grp.OrderByDescending(o => o.LastUpdatedTime).FirstOrDefault();
            //modelProduct.
            var hdvModel = new HistoryDetailViewModel
            {
                History = modelHistory,
                ProductAttributes = modelProduct
            };
            if (modelHistory == null)
            {
                return HttpNotFound();
            }

            return View(hdvModel);
        }

    }
}
