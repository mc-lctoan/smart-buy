using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Models.EntityFramework;
using PagedList;
using System.Data.Entity;


namespace SmartB.UI.Areas.Admin.Controllers
{
    public class UserPriceController : Controller
    {
        //
        // GET: /Admin/UserPrice/
        SmartBuyEntities db = new SmartBuyEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageUserPrice(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            DateTime start = DateTime.Today.AddDays(0);
            DateTime end = DateTime.Today.AddDays(1);
            var userPrice = (from item in db.UserPrices
                            where item.LastUpdatedTime >= start && item.LastUpdatedTime <= end
                            select item).OrderByDescending(u=>u.LastUpdatedTime);
           return View(userPrice.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet, ActionName("UpdateUserPrice")]
        public JsonResult UpdateUserPrice(int id, int productId, int price, DateTime lastUpdatedTime)
        {
            var check = false;
            try
            {
                ProductAttribute product = db.ProductAttributes
                    .Where(p => p.ProductId == productId)
                    .OrderByDescending(p => p.LastUpdatedTime).FirstOrDefault();

                if (product != null && price < product.MinPrice)
                {
                    var proAtt = new ProductAttribute();
                    proAtt.ProductId = productId;
                    proAtt.MinPrice = price;
                    proAtt.MaxPrice = product.MaxPrice;
                    proAtt.LastUpdatedTime = lastUpdatedTime;
                    db.ProductAttributes.Add(proAtt);                    
                }

                UserPrice up = db.UserPrices.Find(id);
                if (up != null)
                {
                    db.UserPrices.Remove(up);
                }
                db.SaveChanges();

                check = true;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            
        }

    }
}
