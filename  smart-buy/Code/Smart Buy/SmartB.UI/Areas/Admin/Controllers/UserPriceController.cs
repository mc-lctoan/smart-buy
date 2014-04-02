using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models.EntityFramework;
using PagedList;
using System.Data.Entity;

namespace SmartB.UI.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "staff")]
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
                            //where item.LastUpdatedTime >= start && item.LastUpdatedTime <= end
                            select item).OrderByDescending(u=>u.LastUpdatedTime);
           return View(userPrice.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet, ActionName("UpdateUserPrice")]
        public JsonResult UpdateUserPrice(int id, int productId, int price, DateTime lastUpdatedTime)
        {
            var check = false;
            string message = "";
            try
            {
                ProductAttribute product = db.ProductAttributes
                    .Where(p => p.ProductId == productId)
                    .OrderByDescending(p => p.LastUpdatedTime).FirstOrDefault();
               
                if (product != null && price < product.MinPrice.GetValueOrDefault())
                {
                    var proAtt = new ProductAttribute();
                    proAtt.ProductId = productId;
                    proAtt.MinPrice = price;
                    proAtt.MaxPrice = product.MaxPrice;
                    proAtt.LastUpdatedTime = lastUpdatedTime;
                    db.ProductAttributes.Add(proAtt);
                    db.SaveChanges();
                }

                UserPrice up = db.UserPrices.Find(id);
                if (up != null)
                {
                    db.UserPrices.Remove(up);
                }
                db.SaveChanges();

                check = true;
                message = "Success";
                TempData["UpdateUserPrice"] = message;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = "Failed";
                TempData["UpdateUserPrice"] = message;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public RedirectToRouteResult Delete(int[] userIds)
        {
            if (userIds != null)
            {
                foreach (var id in userIds)
                {
                    var userPrice = db.UserPrices.FirstOrDefault(x => x.Id == id);
                    if (userPrice != null)
                    {
                        db.UserPrices.Remove(userPrice);
                    }
                }
                db.SaveChanges();
                TempData["deleteUserPrice"] = "Done";
            }
            return RedirectToAction("ManageUserPrice");
        }
    }
}
