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
            var products = from p in db.ProductAttributes
                           where p.Product.Name.Contains(q)
                           group p by p.ProductId into grp
                           select grp.OrderByDescending(o => o.LastUpdatedTime).FirstOrDefault();

            products = products.OrderBy(p => p.Product.Name);

            return View(products.ToPagedList(pageNumber, pageSize));
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
                    if (i < 10)
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
                    else continue;
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

                var minPrice = from p in db.ProductAttributes
                               where p.ProductId == pId
                               select p.MinPrice;

                var maxPrice = from p in db.ProductAttributes
                               where p.ProductId == pId
                               select p.MinPrice;

                var averagePrice = (minPrice.First() + maxPrice.First()) / 2;
                var rangeFrom = minPrice.First() - ep * averagePrice;
                var rangeTo = maxPrice.First() + ep * averagePrice;

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

        //public ActionResult ProductMostBuy()
        //{

        //    var productModel = (from p in db.Products
        //                        from h in db.HistoryDetails
        //                        where h.ProductId == p.Id && h.History.Username.Equals("Sergey Pimenov")
        //                        group h by p into productMostBuyGroup
        //                        select new ProductMostBuy
        //                        {
        //                            Product = productMostBuyGroup.Key,
        //                            numberOfBuy = productMostBuyGroup.Count()
        //                        }).Where(x => x.numberOfBuy >= 5).OrderByDescending(o => o.numberOfBuy).Take(5);

        //    if (productModel.Count() > 0)
        //    {
        //        return PartialView(productModel);
        //    }
        //    else return PartialView();
        //}

        public ActionResult UploadProduct()
        {
            ViewBag.countInsert = TempData["InsertMessage"] as string;
            ViewBag.countUpdate = TempData["UpdateMessage"] as string;
            ViewBag.dupSellProduct = TempData["DictionaryProduct"];
            return View();
        }

        [HttpPost]
        public ActionResult UploadProduct(HttpPostedFileBase excelFile, int? page)
        {

            if (excelFile != null)
            {
                Guid guid = Guid.NewGuid();
                string savedFileName = "~/UploadedExcelFiles/" + guid + excelFile.FileName;
                excelFile.SaveAs(Server.MapPath(savedFileName));

                ExcelHelper excelHelper = new ExcelHelper();
                List<SellProductModel> sellProductCollection = new List<SellProductModel>();
                List<SellProductModel> sellProductCorrectCollection = new List<SellProductModel>();
                List<SellProductModel> sellProductErrorCollection = new List<SellProductModel>();
                ListSellProductModel model = new ListSellProductModel();
                //Catch exception 

                string errorName = "";
                string errorMarket = "";
                string errorPrice = "";
                int errorCount = 0;
                try
                {
                    sellProductCorrectCollection = excelHelper.ReadDataCorrect((Server.MapPath(savedFileName)));
                    sellProductErrorCollection = excelHelper.ReadDataError((Server.MapPath(savedFileName)), out errorName, out errorMarket, out errorPrice, out errorCount);
                    ViewBag.sellProductCorrectCollection = sellProductCorrectCollection;
                    model.CorrectSellProducts = sellProductCorrectCollection;
                    model.InCorrectSellProducts = sellProductErrorCollection;
                    ViewBag.ExceptionName = errorName;
                    ViewBag.ExceptionMarket = errorMarket;
                    ViewBag.ExceptionPrice = errorPrice;
                    ViewBag.errorCount = errorCount;
                    List<string> errorNameLines = new List<string>();
                    List<string> errorMarketNameLines = new List<string>();
                    List<string> errorPriceLines = new List<string>();
                    foreach (var product in sellProductErrorCollection)
                    {
                        var errorNameLine = "";
                        var errorMarketNameLine = "";
                        var errorPriceLine = "";
                        if (product.Name.Length < 5 || product.Name.Length > 100)
                        {
                            errorNameLine += product.RowNumber;
                        }
                        if (product.MarketName.Length < 5 || product.MarketName.Length > 20)
                        {
                            errorMarketNameLine += product.RowNumber;

                        }
                        if (product.Price < 1 || product.Price > 10000)
                        {
                            errorPriceLine += product.RowNumber;
                        }
                        if (errorNameLine != product.Name && errorNameLine != "")
                        {
                            errorNameLines.Add(errorNameLine);
                        }
                        if (errorMarketNameLine != product.MarketName && errorMarketNameLine != "")
                        {
                            errorMarketNameLines.Add(errorMarketNameLine);
                        }
                        if (errorPriceLine != "")
                        {
                            errorPriceLines.Add(errorPriceLine);
                        }
                    }
                    ViewBag.ErrorNameLines = errorNameLines;
                    ViewBag.ErrorMarketNameLines = errorMarketNameLines;
                    ViewBag.ErrorPriceLines = errorPriceLines;
                    TempData["CorrectProducts"] = sellProductCorrectCollection;
                    Session["CorrectProducts"] = sellProductCorrectCollection;
                    ViewBag.Test = "test";
                }
                catch (Exception exception)
                {
                    ViewBag.Exception = exception.Message;
                    ViewBag.ExceptionName = exception.Message;
                    ViewBag.ExceptionMarket = exception.Message;
                    ViewBag.ExceptionPrice = errorPrice;
                    ViewBag.errorCount = errorCount;
                }
                //Compare items in Excel
                List<SellProductModel> compareListProduct = sellProductCorrectCollection;
                List<List<SellProductModel>> results = new List<List<SellProductModel>>();
                for (int i = 0; i < compareListProduct.Count; i++)
                {
                    List<SellProductModel> result = new List<SellProductModel>();
                    for (int j = i + 1; j < compareListProduct.Count; j++)
                    {
                        if (compareListProduct[i].MarketName == compareListProduct[j].MarketName)
                        {
                            var percentage =
                                CompareStringHelper.CompareString(compareListProduct[i].Name.Split(';').First(), compareListProduct[j].Name.Split(';').First());
                            if (percentage > 0.7 && percentage < 1)
                            {
                                if (result.Count() == 0)
                                {
                                    result.Add(compareListProduct[i]);
                                }
                                result.Add(compareListProduct[j]);
                                compareListProduct.Remove(compareListProduct[j]);
                            }
                        }
                    }
                    if (result.Count() != 0)
                    {
                        compareListProduct.Remove(compareListProduct[i]);
                        i = i - 1;
                        results.Add(result);
                    }
                }
                ViewBag.duplicateCorrectProduct = results;
                ViewBag.duplicateCorrectProductCount = results.Count();
                return View(model);
            }
            return View();
        }

        public ActionResult SaveProducts(ListSellProductModel model)
        {
            model.CorrectSellProducts = (List<SellProductModel>)Session["CorrectProducts"];
            var errors = ModelState.Values.Where(x => x.Errors.Count > 0);
            //Trạng thái khi lưu xuống db
            int countUpdate = 0;
            int countInsert = 0;
            List<List<SellProductModel>> dupSellProduct = new List<List<SellProductModel>>();
            foreach (var product in model.CorrectSellProducts)
            {
                SmartBuyEntities db = new SmartBuyEntities();

                //Trùng data
                var productNameFirst = product.Name.Split(';').First(); // Cắt chuỗi 
                var dupMarket = db.Markets.Where(m => m.Name.Equals(product.MarketName)).FirstOrDefault();
                var dupProduct = db.Products.Where(p => p.Name.Equals(productNameFirst)).FirstOrDefault();
                var dupProductDictionary = db.Dictionaries.Where(p => p.Name.Equals(product.Name)).FirstOrDefault();

                //Trung hoan toan
                if (dupMarket != null & dupProduct != null)
                {
                    TrungHoanToan(ref countUpdate, ref countInsert, product, db, dupMarket, dupProduct);

                }
                else if (dupMarket == null & dupProduct != null) // Trùng Tên sản phẩm
                {
                    countInsert = TrungTenSanPham(countInsert, product, db, dupProduct);
                }
                else if (dupMarket != null & dupProduct == null) // Trùng Market
                {
                    var results = TrungMarket(ref countInsert, product, db, productNameFirst, dupMarket, dupProductDictionary);
                    if (results != null)
                    {
                        dupSellProduct.Add(results);
                    }                    
                }
                
                else  //Insert sellProduct mới
                {
                    countInsert = AddNewProduct(countInsert, product, db, productNameFirst, dupProductDictionary);
                }
            }
            if (dupSellProduct.Count() > 0)
            {
                TempData["DictionaryProduct"] = dupSellProduct;
            }
         //   Session["dupProducts"] = dupSellProduct;
            TempData["UpdateMessage"] = "Có " + countUpdate + " sản phẩm được cập nhật giá.";
            TempData["InsertMessage"] = "Có " + countInsert + " sản phẩm được lưu mới.";
            return RedirectToAction("UploadProduct");
        }

        private int AddNewProduct(int countInsert, SellProductModel product, SmartBuyEntities db, string productNameFirst, Dictionary dupProductDictionary)
        {
            #region Comments
            // Check excell với Dictionary
            //List<SellProduct> newCorrectProduct = (List<SellProduct>)Session["CorrectProducts"];
            //List<SellProduct> sellProductCompare = db.SellProducts.ToList();
            //List<List<SellProduct>> results = new List<List<SellProduct>>();
            //for (int i = 0; i < newCorrectProduct.Count; i++)
            //{
            //    List<SellProduct> result = new List<SellProduct>();
            //    for (int j = i + 1; j < sellProductCompare.Count; j++)
            //    {
            //        // var sellProMarket = db.SellProducts.Where(m => m.Market.Equals(sellProductCompare[j].MarketId))
            //        if (newCorrectProduct[i].Market.Name == sellProductCompare[j].Market.Name)
            //        {
            //            var percentage =
            //                CompareStringHelper.CompareString(newCorrectProduct[i].Product.Name.Split(';').First(), sellProductCompare[j].Product.Name);
            //            if (percentage > 0.7 && percentage < 1)
            //            {
            //                // var productSimilarDB = db.Products.Where(p => p.Dictionaries.Equals(dictionaryCompare[j]));
            //                if (result.Count() == 0)
            //                {
            //                    result.Add(newCorrectProduct[i]);
            //                }
            //                result.Add(sellProductCompare[j]);
            //                newCorrectProduct.Remove(newCorrectProduct[i]);
            //            }
            //        }
            //    }
            //    if (result.Count() != 0)
            //    {
            //        i = i - 1;
            //        results.Add(result);
            //    }
            //} 
            #endregion

            var market = new Market
            {
                Name = product.MarketName,
                IsActive = true,
            };
            var newMarket = db.Markets.Add(market); //add market

            var newProduct = new SmartB.UI.Models.EntityFramework.Product
            {
                Name = productNameFirst,
                IsActive = true,
            };
            var addedProduct = db.Products.Add(newProduct); // add product

            var sellProduct = new SmartB.UI.Models.EntityFramework.SellProduct
            {
                Market = newMarket,
                Product = addedProduct,
                SellPrice = product.Price,
                LastUpdatedTime = DateTime.Now
            };
            var addedSellProduct = db.SellProducts.Add(sellProduct); // add sellProduct
            db.SaveChanges(); // Save to database
            //add product Attribute
            var dupProductAtt = db.ProductAttributes.Where(p => p.ProductId.Equals(addedProduct.Id)).FirstOrDefault();
            if (dupProductAtt == null) //không trùng productAtt thì thêm mới
            {
                var productAttribute = new SmartB.UI.Models.EntityFramework.ProductAttribute
                {
                    ProductId = addedProduct.Id,
                    MinPrice = product.Price,
                    MaxPrice = product.Price,
                    LastUpdatedTime = DateTime.Now,
                };
                var addedProductAtt = db.ProductAttributes.Add(productAttribute);
                db.SaveChanges(); // Save to database
            }
            else
            {
                if (product.Price < dupProductAtt.MinPrice)
                {
                    dupProductAtt.MinPrice = product.Price;
                }
                else if (product.Price > dupProductAtt.MaxPrice)
                {
                    dupProductAtt.MaxPrice = product.Price;
                }
                db.SaveChanges(); // Save to database
            }
            countInsert++;
            // add Product Dictionary
            var dictionaries = product.Name.Split(';').ToList();
                foreach (string dictionary in dictionaries)
                {
                    if (dupProductDictionary == null && dictionary != "")
                    {
                        var ProductDic = new SmartB.UI.Models.EntityFramework.Dictionary
                        {
                            Name = dictionary,
                            ProductId = addedProduct.Id
                        };
                        var addProductDic = db.Dictionaries.Add(ProductDic);
                    }
                }
                db.SaveChanges(); // Save to database
                return countInsert;
            
        }

        private static int TrungTenSanPham(int countInsert, SellProductModel product, SmartBuyEntities db, Product dupProduct)
        {
            var dupProductAtt = db.ProductAttributes.Where(p => p.ProductId.Equals(dupProduct.Id)).FirstOrDefault();
            if (product.Price < dupProductAtt.MinPrice)
            {
                dupProductAtt.MinPrice = product.Price;
            }
            else if (product.Price > dupProductAtt.MaxPrice)
            {
                dupProductAtt.MaxPrice = product.Price;
            }
            var market = new Market
            {
                Name = product.MarketName,
                IsActive = true,
            };
            var newMarket = db.Markets.Add(market); //add market

            var sellProduct = new SmartB.UI.Models.EntityFramework.SellProduct
            {
                Market = newMarket,
                Product = dupProduct,
                SellPrice = product.Price,
                LastUpdatedTime = DateTime.Now
            };
            var addedSellProduct = db.SellProducts.Add(sellProduct); // Add SellProduct
            countInsert++;
            db.SaveChanges(); // Save to database
            return countInsert;
        }

        private static List<SellProductModel> TrungMarket(ref int countInsert, SellProductModel product, SmartBuyEntities db, string productNameFirst, Market dupMarket, Dictionary dupProductDictionary)
        {
            
            //Kiem tra productName voi dictionary
            List<SellProductModel> sellProducts = null;
            var productId = CheckProductNameWithDictionary(productNameFirst, db.Dictionaries);
            if (productId != 0)
            {
                var existedSellProduct = db.SellProducts.FirstOrDefault(x => x.ProductId == productId);
                var existedSellProductModel = SellProductModel.MapToSellProductEntity(existedSellProduct);
                sellProducts = new List<SellProductModel>();
                sellProducts.Add(product);
                sellProducts.Add(existedSellProductModel);
            }
            else
            {
                var newProduct = new SmartB.UI.Models.EntityFramework.Product // add Product
                {
                    Name = productNameFirst,
                    IsActive = true,
                };
                var addedProduct = db.Products.Add(newProduct);
                var sellProduct = new SmartB.UI.Models.EntityFramework.SellProduct //add SellProduct
                {
                    Market = dupMarket,
                    Product = addedProduct,
                    SellPrice = product.Price,
                    LastUpdatedTime = DateTime.Now
                };
                var addedSellProduct = db.SellProducts.Add(sellProduct);
                countInsert++;
                db.SaveChanges(); // Save to database
                //add new product Attribute                       

                var productAttribute = new SmartB.UI.Models.EntityFramework.ProductAttribute
                {
                    ProductId = addedProduct.Id,
                    MinPrice = product.Price,
                    MaxPrice = product.Price,
                    LastUpdatedTime = DateTime.Now,
                };
                var addedProductAtt = db.ProductAttributes.Add(productAttribute);
                db.SaveChanges(); // Save to database
                // add Product Dictionary
                var dictionaries = product.Name.Split(';').ToList();
                    foreach (string dictionary in dictionaries)
                    {
                        if (dupProductDictionary == null && dictionary != "")
                        {
                            var ProductDic = new SmartB.UI.Models.EntityFramework.Dictionary
                            {
                                Name = dictionary,
                                ProductId = addedProduct.Id
                            };
                            var addProductDic = db.Dictionaries.Add(ProductDic);
                        }
                    }

                    db.SaveChanges(); // Save to database
                
            }
            return sellProducts;
        }

        private static int? CheckProductNameWithDictionary(string productNameFirst, DbSet<Dictionary> dictionaries)
        {
            foreach (var dictionary in dictionaries.ToList())
            {
                if (CompareStringHelper.CompareString(dictionary.Name, productNameFirst) > 0.85)
                {
                    return dictionary.ProductId;
                }
            }
            return 0;
        }

        private static void TrungHoanToan(ref int countUpdate, ref int countInsert, SellProductModel product, SmartBuyEntities db, Market dupMarket, Product dupProduct)
        {
            var sellProduct = db.SellProducts.Where(s => s.ProductId == dupProduct.Id && s.MarketId == dupMarket.Id).FirstOrDefault();
            // Check sellProduct có trùng không??
            if (sellProduct == null)
            {
                var sellProduct1 = new SmartB.UI.Models.EntityFramework.SellProduct //add SellProduct
                {
                    Market = dupMarket,
                    Product = dupProduct,
                    SellPrice = product.Price,
                    LastUpdatedTime = DateTime.Now
                };
                var addedSellProduct = db.SellProducts.Add(sellProduct1);

                countInsert++;
                db.SaveChanges(); // Save to database
            }
            else
            {
                if (sellProduct.SellPrice != product.Price)
                {
                    sellProduct.SellPrice = product.Price;
                    countUpdate++;
                }
                else
                {
                    sellProduct.SellPrice = product.Price;
                }
            }
            // Cập nhật giá Min, Max ProductAttribute
            var dupProductAtt = db.ProductAttributes.Where(p => p.ProductId.Equals(dupProduct.Id)).FirstOrDefault();
            if (product.Price < dupProductAtt.MinPrice)
            {
                dupProductAtt.MinPrice = product.Price;
            }
            else if (product.Price > dupProductAtt.MaxPrice)
            {
                dupProductAtt.MaxPrice = product.Price;
            }
            db.SaveChanges(); // Save to database
        }



        public JsonResult SaveProductError(string ProductId, string ProductName, string ProductMarketName, int ProductPrice)
        {
            Result result = new Result();
            List<string> error = new List<string>();

            if (ProductName.Length < 5 || ProductName.Length > 100)
            {
                error.Add("Tên sản phẩm phải từ 5 đến 100 ký tự");
            }
            if (ProductMarketName.Length < 5 || ProductMarketName.Length > 20)
            {
                error.Add("Tên chợ phải từ 5 đến 20 ký tự");
            }
            if (ProductPrice < 1 || ProductPrice > 10000)
            {
                error.Add("Giá phải từ 1 đến 10000");
            }
            if (error.Count == 0)
            {
                var correctProducts = (List<SellProductModel>)Session["CorrectProducts"];
                var existedProduct = correctProducts.FirstOrDefault(x => x.Name.Split(';').First() == ProductName && x.MarketName == ProductMarketName);
                if (existedProduct != null)
                {
                    error.Add("Sản phẩm đã có.");
                    result.id = existedProduct.Id;
                    result.updatedPrice = ProductPrice;
                }
                else
                {
                    var largerId = correctProducts.OrderByDescending(p => p.Id).FirstOrDefault();
                    int newId = largerId.Id + 1;
                    SellProductModel model = new SellProductModel();
                    model.Name = ProductName;
                    model.MarketName = ProductMarketName;
                    model.Price = ProductPrice;
                    model.Id = newId;
                    correctProducts.Add(model);
                    Session["CorrectProducts"] = correctProducts;
                    result.id = newId;
                }
            }

            result.error = error;
            return Json(result);
        }

        public class Result
        {
            public List<string> error { get; set; }
            public int id { get; set; }
            public int updatedPrice { get; set; }
        }

        [HttpDelete]
        public JsonResult DeleteProduct(int id)
        {
            var result = true;
            var correctProducts = (List<SellProductModel>)Session["CorrectProducts"];
            var removeProduct = correctProducts.FirstOrDefault(x => x.Id == id);
            if (removeProduct != null)
            {
                correctProducts.Remove(removeProduct);
            }
            else
            {
                result = false;
            }
            Session["CorrectProducts"] = correctProducts;
            return Json(result);

        }

        public JsonResult UpdateSession(string ProductId, string ProductName, string ProductMarketName, int ProductPrice)
        {

            var correctProducts = (List<SellProductModel>)Session["CorrectProducts"];
            var largerId = correctProducts.OrderByDescending(p => p.Id).FirstOrDefault();
            int newId = largerId.Id + 1;
            SellProductModel model = new SellProductModel();
            model.Name = ProductName;
            model.MarketName = ProductMarketName;
            model.Price = ProductPrice;
            model.Id = largerId.Id + 1;
            correctProducts.Add(model);
            Session["CorrectProducts"] = correctProducts;
            return Json(newId);
        }
        public JsonResult MergeProduct(string ProductId, string ProductName, string ProductMarketName, int ProductPrice)
        {
            var correctProducts = (List<SellProductModel>)Session["CorrectProducts"];
            var largerId = correctProducts.OrderByDescending(p => p.Id).FirstOrDefault();
            if (largerId == null)
            {
                largerId.Id = 0;
            }
            int newId = largerId.Id + 1;
            SellProductModel model = new SellProductModel();
            model.Name = ProductName;
            model.MarketName = ProductMarketName;
            model.Price = ProductPrice;
            model.Id = largerId.Id + 1;
            correctProducts.Add(model);
            
            Session["CorrectProducts"] = correctProducts;
            return Json(newId);
        }
    }
}
