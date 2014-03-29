using SmartB.UI.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.Script.Services;

namespace SmartB.UI.Areas.Admin.Controllers
{
    public class ManageProductController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();
        private const int PageSize = 10;
        //
        // GET: /Admin/ManageProduct/

        public ActionResult Index(int page = 1)
        {
            var sellProducts = context.SellProducts
                .OrderBy(x => x.Product.Name)
                .ToPagedList(page, PageSize);
            return View(sellProducts);
        }
        public ActionResult Create()
        {
            //bind drop down list
            var market = from m in context.Markets
                         orderby m.Name
                         select m;
            ViewBag.ddlMarket = new SelectList(market, "Id", "Name");
            return View();
        }

        public JsonResult SaveSellProduct(string productName, int marketId, int sellPrice)
        {
            try
            {
                // Not exist
                Product product = context.Products.FirstOrDefault(x => x.Name.Equals(productName));
                var market = context.Markets.Where(m => m.Id.Equals(marketId)).FirstOrDefault();
                if (product != null)
                {
                    var sellProduct = context.SellProducts
                        .FirstOrDefault(x => x.ProductId == product.Id && x.MarketId == marketId);
                    if (sellProduct != null)
                    {
                        TempData["create"] = "Duplicate";
                    }
                }
                else
                {
                    // add product
                    var newProduct = new SmartB.UI.Models.EntityFramework.Product
                    {
                        Name = productName,
                        IsActive = true,
                    };
                    var addedProduct = context.Products.Add(newProduct);

                    var newSellProduct = new SmartB.UI.Models.EntityFramework.SellProduct //add SellProduct
                    {
                        Market = market,
                        Product = addedProduct,
                        SellPrice = sellPrice,
                        LastUpdatedTime = DateTime.Now
                    };
                    var addedSellProduct = context.SellProducts.Add(newSellProduct);
                    context.SaveChanges(); // Save to database

                    var productAttribute = new SmartB.UI.Models.EntityFramework.ProductAttribute
                    {
                        ProductId = addedProduct.Id,
                        MinPrice = sellPrice,
                        MaxPrice = sellPrice,
                        LastUpdatedTime = DateTime.Now,
                    };
                    var addedProductAtt = context.ProductAttributes.Add(productAttribute);
                    context.SaveChanges(); // Save to database
                    // add Product Dictionary
                    var dictionaries = productName.Split(';').ToList();
                    var dupProductDictionary = context.Dictionaries.Where(p => p.Name.Equals(productName)).FirstOrDefault();
                    foreach (string dictionary in dictionaries)
                    {
                        if (dupProductDictionary == null && dictionary != "")
                        {
                            var ProductDic = new SmartB.UI.Models.EntityFramework.Dictionary
                            {
                                Name = dictionary,
                                ProductId = addedProduct.Id
                            };
                            var addProductDic = context.Dictionaries.Add(ProductDic);
                        }
                    }

                    context.SaveChanges(); // Save to database

                    TempData["create"] = "Success";
                }
               return Json(JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult Edit(int id)
        {
            var sellProduct = context.SellProducts.FirstOrDefault(x => x.Id == id);
            //bind drop down list
            var market = from m in context.Markets
                         orderby m.Name
                         select m;
            ViewBag.ddlMarket = new SelectList(market, "Id", "Name");
            return View(sellProduct);
        }

        [HttpPost]
        public RedirectToRouteResult Edit(SellProduct model)
        {
            Product product = context.Products.FirstOrDefault(x => x.Name.Equals(model.Product.Name));
            var sellProduct = context.SellProducts.FirstOrDefault(x => x.ProductId == product.Id && x.Market.Id == model.MarketId);
            var dupSellProduct = context.SellProducts.FirstOrDefault(x => x.ProductId == product.Id && x.Market.Id == model.MarketId && x.SellPrice == model.SellPrice);
            string message;
            if (dupSellProduct != null)
            {
                message = "Duplicate";
            } 
            else if (sellProduct != null)
            {
                sellProduct.SellPrice = model.SellPrice;
                sellProduct.LastUpdatedTime = System.DateTime.Now;
                context.SaveChanges();
                message = "Success";
            }
            else if (sellProduct == null)
            {
                    // add product
                    var newProduct = new SmartB.UI.Models.EntityFramework.Product
                    {
                        Name = model.Product.Name,
                        IsActive = true,
                    };
                    var addedProduct = context.Products.Add(newProduct);

                    sellProduct.ProductId = addedProduct.Id;
                    sellProduct.MarketId = model.MarketId;
                    sellProduct.SellPrice = model.SellPrice;
                    sellProduct.LastUpdatedTime = System.DateTime.Now;
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
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var sellProduct = context.SellProducts.FirstOrDefault(x => x.Id == id);
                    if (sellProduct != null)
                    {
                        context.SellProducts.Remove(sellProduct);
                    }
                }
                context.SaveChanges();
                TempData["delete"] = "Done";
            }
            else
            {
                TempData["delete"] = "Empty";
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string> GetProductName(string pre)
        {
            SmartBuyEntities context = new SmartBuyEntities();
            List<string> allCompanyName = new List<string>();
            
                allCompanyName = (from a in context.Products
                                  where a.Name.StartsWith(pre)
                                  select a.Name).ToList();
            return allCompanyName;
        }
    }
}
