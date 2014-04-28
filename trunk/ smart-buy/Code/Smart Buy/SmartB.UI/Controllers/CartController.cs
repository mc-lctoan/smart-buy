using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        private SmartBuyEntities db = new SmartBuyEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ViewResult ViewCart()
        {
            if (GetCart().Lines.Count() == 0)
            {
                return View();
            }
            return View(new CartViewModel
            {
                Cart = GetCart()
            });
        }

        [HttpGet, ActionName("AddToCart")]
        public JsonResult AddToCart(int productId)
        {
            if (GetCart().Lines.Count() >= 20)
            {
                return Json("full", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var check = false;
                try
                {
                    ProductAttribute product = db.ProductAttributes
                        .Where(p => p.ProductId == productId)
                        .OrderByDescending(p => p.LastUpdatedTime).FirstOrDefault();

                    if (product != null)
                    {
                        GetCart().AddItem(product, 1);
                    }
                    check = true;
                    return Json(check, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(check, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet, ActionName("RemoveFromCart")]
        public JsonResult RemoveFromCart(int productId)
        {
            var check = false;
            try
            {
                ProductAttribute product = db.ProductAttributes
                        .Where(p => p.ProductId == productId)
                        .OrderByDescending(p => p.LastUpdatedTime).FirstOrDefault();

                if (product != null)
                {
                    GetCart().RemoveLine(product);
                }
                check = true;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(check, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet, ActionName("UpdateItemInCart")]
        public JsonResult UpdateItemInCart(int productId, float quantity)
        {
            var check = false;
            try
            {
                ProductAttribute product = db.ProductAttributes
                        .Where(p => p.ProductId == productId)
                        .OrderByDescending(p => p.LastUpdatedTime).FirstOrDefault();

                if (product != null)
                {
                    GetCart().UpdateItem(product, quantity);
                }
                string totalPrice = Math.Floor(GetCart().ComputeTotalMin()) + " - " + Math.Floor(GetCart().ComputeTotalMax());

                return Json(totalPrice);
            }
            catch
            {
                return Json(check, JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult Summary()
        {
            return PartialView(GetCart());
        }

        public ActionResult SaveCart()
        {
            if (GetCart().Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Không có sản phẩm nào trong giỏ hàng.");
            }

            if (ModelState.IsValid)
            {
                var now = DateTime.Now.Date;
                foreach (var line in GetCart().Lines)
                {
                    var pid = line.Product.ProductId;

                    var dupHistory = db.Histories.FirstOrDefault(his => his.Username == User.Identity.Name &&
                                                                        his.BuyTime == now);

                    if (dupHistory == null)
                    {
                        var newHitory = new History();
                        newHitory.Username = User.Identity.Name;
                        newHitory.BuyTime = now;

                        db.Histories.Add(newHitory);
                        var newHistoryDetail = new HistoryDetail();
                        newHistoryDetail.History = newHitory;
                        newHistoryDetail.ProductId = pid;
                        newHistoryDetail.MinPrice = line.Product.MinPrice;
                        newHistoryDetail.MaxPrice = line.Product.MaxPrice;
                        db.HistoryDetails.Add(newHistoryDetail);

                    }
                    else
                    {
                        var historyId = (from h in db.Histories
                                         where h.BuyTime == now && h.Username == User.Identity.Name
                                         select h.Id).First();
                        //var checkCount = (from c in db.HistoryDetails
                        //                  where c.HistoryId == historyId
                        //                  select c).Count();
                        //if (checkCount >= 10)
                        //{
                        //    break;
                        //}

                        var dupProductId = db.HistoryDetails.FirstOrDefault(p => p.ProductId == pid && p.HistoryId == historyId);
                        if (dupProductId == null)
                        {
                            var newHistoryDetail = new HistoryDetail();
                            newHistoryDetail.History = dupHistory;
                            newHistoryDetail.ProductId = pid;
                            newHistoryDetail.MinPrice = line.Product.MinPrice;
                            newHistoryDetail.MaxPrice = line.Product.MaxPrice;
                            db.HistoryDetails.Add(newHistoryDetail);
                        }
                    }
                    db.SaveChanges();

                }
                GetCart().Clear();
            }

            return RedirectToAction("BuyingHistory", "History");
        }

        private Cart GetCart()
        {

            Cart cart = (Cart)Session["Cart"];
            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
    }
}
