using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models.MobileModel;
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
                            where item.isApprove == false
                            select item).OrderByDescending(u=>u.LastUpdatedTime);
           return View(userPrice.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ManageUserPriceApprove(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            DateTime start = DateTime.Today.AddDays(0);
            DateTime end = DateTime.Today.AddDays(1);
            var userPrice = (from item in db.UserPrices
                             where item.isApprove == true
                             select item).OrderByDescending(u => u.LastUpdatedTime);
            return View(userPrice.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet, ActionName("UpdateUserPrice")]
        public JsonResult UpdateUserPrice(int id, int productId, int price, DateTime lastUpdatedTime, int marketId)
        {
            var check = false;
            string message = "";
            try
            {
                ProductAttribute product = db.ProductAttributes
                    .Where(p => p.ProductId == productId)
                    .OrderByDescending(p => p.LastUpdatedTime).FirstOrDefault();
                //var getSellProduct = db.SellProducts.Where(s => s.Product.Id == productId & s.MarketId == marketId).FirstOrDefault();
                var sellProduct = new SellProduct();
                sellProduct.MarketId = marketId;
                sellProduct.ProductId = productId;
                sellProduct.SellPrice = price;
                sellProduct.LastUpdatedTime = lastUpdatedTime;
                db.SellProducts.Add(sellProduct);
                
                if (product != null && price < product.MinPrice.GetValueOrDefault())
                {
                    //if (getSellProduct != null)
                    //{
                    //    getSellProduct.SellPrice = price; //update Price
                    //}
                    var proAtt = new ProductAttribute();
                    proAtt.ProductId = productId;
                    proAtt.MinPrice = price;
                    proAtt.MaxPrice = product.MaxPrice;
                    proAtt.LastUpdatedTime = lastUpdatedTime;
                    db.ProductAttributes.Add(proAtt);
                }

                else if (product != null && price > product.MaxPrice.GetValueOrDefault())
                {
                    //if (getSellProduct != null)
                    //{
                    //    getSellProduct.SellPrice = price; //update Price
                    //}
                    
                    var proAtt = new ProductAttribute();
                    proAtt.ProductId = productId;
                    proAtt.MinPrice = product.MinPrice;
                    proAtt.MaxPrice = price;
                    proAtt.LastUpdatedTime = lastUpdatedTime;
                    db.ProductAttributes.Add(proAtt);
                }

                UserPrice up = db.UserPrices.Find(id);
                if (up != null)
                {
                    up.isApprove = true;
                }
                db.SaveChanges();

                UpdateUserPriceModel.checkApprove = true;

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

        [HttpPost]
        public RedirectToRouteResult DeleteApprove(int[] userIds)
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
            return RedirectToAction("ManageUserPriceApprove");
        }
    }
}
