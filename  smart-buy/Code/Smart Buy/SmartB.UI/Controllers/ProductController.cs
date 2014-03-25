using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Models.EntityFramework;
using System.Data.Entity;
using SmartB.UI.Models;
using SmartB.UI.UploadedExcelFiles;
using System.Web.Script.Serialization;
using SmartB.UI.Areas.Admin.Helper;
using PagedList;
using System.Net;
using SmartB.UI.Areas.Admin.Models;
using System.IO;
using System.Reflection;

namespace SmartB.UI.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        private SmartBuyEntities db = new SmartBuyEntities();
        public ActionResult SearchProduct(string q, string currentFilter, int? page)
        {

            if (String.IsNullOrEmpty(q) && String.IsNullOrEmpty(currentFilter))
            {
                return View();
            }
            if (!String.IsNullOrEmpty(q))
            {
                page = 1;
            }
            else
            {
                q = currentFilter;
            }

            ViewBag.CurrentFilter = q;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var dictionaries = db.Dictionaries.Include(i => i.Product).Where(p => p.Name.Contains(q)).ToList();

            var result = new List<ProductInfo>();
            foreach (Dictionary dictionary in dictionaries)
            {
                List<int?> minPrice = dictionary.Product.ProductAttributes
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .Select(x => x.MinPrice)
                    .ToList();
                List<int?> maxPrice = dictionary.Product.ProductAttributes
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .Select(x => x.MaxPrice)
                    .ToList();
                var lastUpdated = db.ProductAttributes.Where(p => p.Id == dictionary.ProductId).Select(p => p.LastUpdatedTime).FirstOrDefault();
                var productName = db.Products.Where(p => p.Id == dictionary.ProductId).Select(p => p.Name).FirstOrDefault();
                if (!result.Any(p => p.Name == productName))
                {
                    var info = new ProductInfo
                    {
                        ProductId = dictionary.ProductId.GetValueOrDefault(),
                        Name = productName,
                        MinPrice = minPrice[0].Value,
                        MaxPrice = maxPrice[0].Value,
                        LastUpdatedTime = Convert.ToDateTime(lastUpdated),
                    };
                    result.Add(info);
                }

            }

            //var products = from p in db.Products
            //               where p.ProductId == productID
            //               group p by p.ProductId into grp
            //               select grp.OrderByDescending(o => o.LastUpdatedTime).FirstOrDefault();

            //products = products.OrderBy(p => p.Product.Name);

            //return View(result.ToPagedList(pageNumber, pageSize));
            return View(result);
        }

        public ActionResult ViewCart()
        {
            return View();
        }

        [HttpGet, ActionName("SaveCart")]
        public JsonResult SaveCart(String totaldata)
        {
            var check = false;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<CartHistory> dataList = ser.Deserialize<List<CartHistory>>(totaldata);
            try
            {
                for (int i = 0; i < dataList.Count; i++)
                {
                    var username = dataList[i].Username;
                    var pid = dataList[i].ProductId;
                    var now = DateTime.Now.Date;
                    var dupHistory = db.Histories.Where(his => his.Username == username &&
                        his.BuyTime == now).FirstOrDefault();



                    if (dupHistory == null)
                    {
                        var newHitory = new History();
                        newHitory.Username = username;
                        newHitory.BuyTime = now;

                        db.Histories.Add(newHitory);
                        var newHistoryDetail = new HistoryDetail();
                        newHistoryDetail.History = newHitory;
                        newHistoryDetail.ProductId = pid;
                        newHistoryDetail.MinPrice = dataList[i].MinPrice;
                        newHistoryDetail.MaxPrice = dataList[i].MaxPrice;
                        db.HistoryDetails.Add(newHistoryDetail);

                    }
                    else
                    {
                        var historyId = (from h in db.Histories
                                         where h.BuyTime == now && h.Username == username
                                         select h.Id).First();
                        var checkCount = (from c in db.HistoryDetails
                                          where c.HistoryId == historyId
                                          select c).Count();
                        if (checkCount >= 10)
                        {
                            return Json("full", JsonRequestBehavior.AllowGet);
                        }
                        var dupProductId = db.HistoryDetails.Where(p => p.ProductId == pid && p.HistoryId == historyId).FirstOrDefault();
                        if (dupProductId == null)
                        {
                            var newHistoryDetail = new HistoryDetail();
                            newHistoryDetail.History = dupHistory;
                            newHistoryDetail.ProductId = pid;
                            newHistoryDetail.MinPrice = dataList[i].MinPrice;
                            newHistoryDetail.MaxPrice = dataList[i].MaxPrice;
                            db.HistoryDetails.Add(newHistoryDetail);

                        }
                    }
                    db.SaveChanges();


                }

                check = true;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            //return RedirectToAction("SearchProduct");
        }

        public ActionResult ProposeProductPrice(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var modelProduct = db.Products.Include(i => i.ProductAttributes).Single(s => s.Id == id);

            if (modelProduct == null)
            {
                return HttpNotFound();
            }

            //bind drop down list
            var market = from m in db.Markets
                         orderby m.Name
                         select m;
            ViewBag.ddlMarket = new SelectList(market, "Id", "Name");
            //ViewBag.searchKey = q;

            return PartialView(modelProduct);
        }

        [HttpGet, ActionName("SaveUserPrice")]
        public JsonResult SaveUserPrice(String userPriceJson)
        {
            var check = false;

            // define epsilon
            var ep = 0.1;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            UserPrice parseJson = ser.Deserialize<UserPrice>(userPriceJson);
            try
            {
                var pId = parseJson.ProductId;
                var updatedPrice = parseJson.UpdatedPrice;

                var minPrice = db.ProductAttributes.Where(p => p.ProductId == pId)
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .Select(x => x.MinPrice)
                    .FirstOrDefault();


                var maxPrice = db.ProductAttributes.Where(p => p.ProductId == pId)
                    .OrderByDescending(x => x.LastUpdatedTime)
                    .Select(x => x.MaxPrice)
                    .FirstOrDefault();

                var averagePrice = (minPrice + maxPrice) / 2;
                var rangeFrom = minPrice - ep * averagePrice;
                var rangeTo = maxPrice + ep * averagePrice;

                if (updatedPrice >= rangeFrom && updatedPrice <= rangeTo)
                {
                    var userPrice = new UserPrice();
                    userPrice.Username = parseJson.Username;
                    userPrice.MarketId = parseJson.MarketId;
                    userPrice.ProductId = pId;
                    userPrice.UpdatedPrice = updatedPrice;
                    userPrice.LastUpdatedTime = DateTime.Now;
                    db.UserPrices.Add(userPrice);
                    db.SaveChanges();
                }

                check = true;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(check, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ProductMostBuy()
        {
            if (Session["Username"] != null)
            {
                string userName = Session["Username"].ToString();
                var productModel = (from p in db.Products
                                    from h in db.HistoryDetails
                                    where h.ProductId == p.Id && h.History.Username.Equals(userName)
                                    group h by p into productMostBuyGroup
                                    select new ProductMostBuy
                                    {
                                        Product = productMostBuyGroup.Key,
                                        numberOfBuy = productMostBuyGroup.Count()
                                    }).Where(x => x.numberOfBuy >= 5).OrderByDescending(o => o.numberOfBuy).Take(5);

                if (productModel.Count() > 0)
                {
                    return PartialView(productModel);
                }
            }
            return PartialView();
        }
    }
}
