using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Areas.Admin.Helper;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;
using SmartB.UI.UploadedExcelFiles;
using PagedList;

namespace SmartB.UI.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "staff")]
    public class ImportController : Controller
    {
        //
        // GET: /Admin/Import/
        private const int PageSize = 10;
        private SmartBuyEntities db = new SmartBuyEntities();
        public ActionResult UploadProduct()
        {
            ViewBag.countInsert = TempData["InsertMessage"] as string;
            ViewBag.countUpdate = TempData["UpdateMessage"] as string;
            ViewBag.dupSellProduct = TempData["DictionaryProduct"];
            return View();
        }

        [HttpPost]
        public ActionResult UploadProduct(HttpPostedFileBase excelFile)
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
                    model.CorrectSellProducts = sellProductCorrectCollection;
                    model.InCorrectSellProducts = sellProductErrorCollection;
                    model.ExceptionName = errorName;
                    model.ExceptionMarket = errorMarket;
                    model.ExceptionPrice = errorPrice;
                    model.ErrorCount = errorCount;
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
                        if (product.MarketName.Length < 5 || product.MarketName.Length > 100)
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
                    model.ErrorNameLines = errorNameLines;
                    model.ErrorMarketNameLines = errorMarketNameLines;
                    model.ErrorPriceLines = errorPriceLines;
                    TempData["CorrectProducts"] = sellProductCorrectCollection;
                    Session["CorrectProducts"] = sellProductCorrectCollection;
                }
                catch (Exception exception)
                {
                    model.Exception = exception.Message;
                    model.ExceptionName = exception.Message;
                    model.ExceptionMarket = exception.Message;
                    model.ExceptionPrice = errorPrice;
                    model.ErrorCount = errorCount;
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
                model.duplicateCorrectProduct = results;
                model.duplicateCorrectProductCount = results.Count();
                Session["duplicateProducts"] = results;
                if (model.CorrectSellProducts != null)
                {
                    model.PagedCorrectProducts = model.CorrectSellProducts.OrderBy(x => x.Name).ToPagedList(1, PageSize);
                }
                Session["excel"] = model;

                return View(model);
            }

            return View();
        }

        public ActionResult PagedData(int page)
        {
            var model = (ListSellProductModel)Session["excel"];
            model.PagedCorrectProducts = model.CorrectSellProducts.OrderBy(x => x.Name).ToPagedList(page, PageSize);
            return View("UploadProduct", model);
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
            Session.Remove("CorrectProducts");
            Session.Remove("duplicateProducts");
            Session["duplicateProducts"] = dupSellProduct;
            TempData["UpdateMessage"] = "Có " + countUpdate + " sản phẩm được cập nhật giá.";
            TempData["InsertMessage"] = "Có " + countInsert + " sản phẩm được lưu mới.";
            return RedirectToAction("UploadProduct");
        }
        public ActionResult SaveDupProducts(ListSellProductModel model)
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
                        if (dictionary != "")
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

                else  //Insert sellProduct mới
                {
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
                        if (dictionary != "")
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
            }

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
                sellProducts.Add(existedSellProductModel);
                sellProducts.Add(product);
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
                    sellProduct.LastUpdatedTime = System.DateTime.Now;
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
                dupProductAtt.LastUpdatedTime = System.DateTime.Now;
            }
            else if (product.Price > dupProductAtt.MaxPrice)
            {
                dupProductAtt.MaxPrice = product.Price;
                dupProductAtt.LastUpdatedTime = System.DateTime.Now;
            }
            dupProductAtt.LastUpdatedTime = System.DateTime.Now;
            db.SaveChanges(); // Save to database
        }

        public JsonResult SaveProductError(string ProductId, string ProductName, string ProductMarketName, int ProductPrice)
        {
            Result result = new Result();
            List<string> error = new List<string>();
            var status = "";
            int tableId = 0;
            var nameDupProduct = "";
            int tableCorrectId = 0;
            var correctProductName = "";
            var correctMarketName = "";
            int correctProductPrice = 0;
            int count = 0;
            if (ProductName.Length < 5 || ProductName.Length > 100)
            {
                error.Add("Tên sản phẩm phải từ 5 đến 100 ký tự");
            }
            if (ProductMarketName.Length < 5 || ProductMarketName.Length > 100)
            {
                error.Add("Tên chợ phải từ 5 đến 100 ký tự");
            }
            if (ProductPrice < 1 || ProductPrice > 10000)
            {
                error.Add("Giá phải từ 1 đến 10000");
            }
            if (error.Count == 0)
            {
                var correctProducts = (List<SellProductModel>)Session["CorrectProducts"];
                var dupCorrectProducts = (List<List<SellProductModel>>)Session["duplicateProducts"];
                var compareResult = false;

                // Compare with duplicate Products
                for (int i = 0; i < dupCorrectProducts.Count; i++)
                {
                    if (compareResult == true)
                    {
                        tableId = i;
                        status = "Duplicate Products List";
                        break;
                    }
                    for (int j = 0; j < dupCorrectProducts[j].Count; j++)
                    {
                        if (ProductMarketName == dupCorrectProducts[i][j].MarketName)
                        {
                            var percentage =
                                CompareStringHelper.CompareString(ProductName.Split(';').First(), dupCorrectProducts[i][j].Name.Split(';').First());
                            if (percentage > 0.7 && percentage < 1)
                            {
                                var largerId = correctProducts.OrderByDescending(p => p.Id).FirstOrDefault();
                                int newId = largerId.Id + 1;
                                SellProductModel model = new SellProductModel();
                                model.Name = ProductName;
                                model.MarketName = ProductMarketName;
                                model.Price = ProductPrice;
                                model.Id = newId;
                                dupCorrectProducts[i].Add(model);
                                Session["duplicateProducts"] = dupCorrectProducts;
                                result.id = newId;
                                nameDupProduct = dupCorrectProducts[i][j].Name;
                                compareResult = true;
                                break;
                            }
                        }
                    }
                }

                // Compare with Correct Products
                if (compareResult == false)
                {
                    var compareCorrectResult = false;
                    if (compareCorrectResult == false)
                    {
                        for (int k = 0; k < correctProducts.Count; k++)
                        {
                            List<SellProductModel> resultCompareError = new List<SellProductModel>();
                            if (ProductMarketName == correctProducts[k].MarketName)
                            {
                                var percentage =
                                        CompareStringHelper.CompareString(ProductName.Split(';').First(), correctProducts[k].Name.Split(';').First());
                                if (percentage > 0.7 && percentage < 1)
                                {
                                    var largerId = correctProducts.OrderByDescending(p => p.Id).FirstOrDefault();
                                    int newId = largerId.Id + 1;
                                    SellProductModel model = new SellProductModel();
                                    model.Name = ProductName;
                                    model.MarketName = ProductMarketName;
                                    model.Price = ProductPrice;
                                    model.Id = newId;
                                    resultCompareError.Add(model);
                                    resultCompareError.Add(correctProducts[k]);
                                    result.id = newId;
                                    compareCorrectResult = true;
                                }
                            }
                            if (compareCorrectResult == true)
                            {
                                tableCorrectId = k;
                                correctProductName = correctProducts[k].Name;
                                correctMarketName = correctProducts[k].MarketName;
                                correctProductPrice = correctProducts[k].Price;
                                nameDupProduct = correctProducts[k].Name;
                                correctProducts.Remove(correctProducts[k]); // Remove product in Correct Products
                                dupCorrectProducts.Add(resultCompareError);
                                Session["duplicateProducts"] = dupCorrectProducts;
                                count = dupCorrectProducts.Count;
                                status = "Correct Products List";
                                break;
                            }
                        }
                    }

                    if (compareCorrectResult == false)
                    {
                        // So sánh với Correct Products xem có bị trùng không
                        for (int l = 0; l < correctProducts.Count; l++)
                        {
                            if (ProductMarketName == correctProducts[l].MarketName)
                            {
                                var percentage =
                                        CompareStringHelper.CompareString(ProductName.Split(';').First(), correctProducts[l].Name.Split(';').First());
                                if (percentage == 1)
                                {
                                    error.Add("Sản phẩm đã có.");
                                    result.id = correctProducts[l].Id;
                                    result.updatedPrice = ProductPrice;
                                    compareCorrectResult = true;
                                    break;
                                }
                            }
                        }
                        if (compareCorrectResult == false)
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
                }
            }
            result.status = status;
            result.tableId = tableId;
            result.nameDupProduct = nameDupProduct;
            result.tableCorrectId = tableCorrectId;
            result.correctProductName = correctProductName;
            result.correctMarketName = correctMarketName;
            result.correctProductPrice = correctProductPrice;
            result.count = count;
            result.error = error;

            return Json(result);
        }

        public class Result
        {
            public List<string> error { get; set; }
            public int id { get; set; }
            public int updatedPrice { get; set; }
            public string status { get; set; }
            public int tableId { get; set; }
            public int tableCorrectId { get; set; }
            public string nameDupProduct { get; set; }
            public string correctProductName { get; set; }
            public string correctMarketName { get; set; }
            public int correctProductPrice { get; set; }
            public int count { get; set; }
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
            Result result = new Result();
            if (Session["CorrectProducts"] != null)  // Khi chưa so sánh với DB
            {
                var correctProducts = (List<SellProductModel>)Session["CorrectProducts"];
                var largerId = correctProducts.OrderByDescending(p => p.Id).FirstOrDefault();
                SellProductModel model = new SellProductModel();
                model.Name = ProductName;
                model.MarketName = ProductMarketName;
                model.Price = ProductPrice;
                model.Id = largerId.Id + 1;
                correctProducts.Add(model);
                Session["CorrectProducts"] = correctProducts;

                var dupCorrectProducts = (List<List<SellProductModel>>)Session["duplicateProducts"];
                string[] productNames = ProductName.Split(';');
                for (int h = 0; h < productNames.Count(); h++)
                {
                    var status = false;
                    if (productNames[h].ToString() != "")
                    {
                        for (int i = 0; i < dupCorrectProducts.Count; i++)
                        {
                            if (status == true)
                            {
                                break;
                            }
                            for (int j = 0; j < dupCorrectProducts[i].Count; j++)
                            {
                                var nameDupProduct = dupCorrectProducts[i][j].Name;
                                if (productNames[h].ToString() == nameDupProduct.ToString())
                                {
                                    dupCorrectProducts[i].Remove(dupCorrectProducts[i][j]);
                                    status = true;
                                    break;
                                }
                                Session["duplicateProducts"] = dupCorrectProducts;
                            }

                            if (dupCorrectProducts[i].Count == 0)
                            {
                                dupCorrectProducts.Remove(dupCorrectProducts[i]);
                            }
                        }
                    }
                }

                result.id = largerId.Id + 1;
                result.correctProductName = ProductName;
                result.correctMarketName = ProductMarketName;
                result.correctProductPrice = ProductPrice;
            }
            else // So sánh với DB
            {
                List<SellProductModel> correctProductsCollection = new List<SellProductModel>();
                SellProductModel model = new SellProductModel();
                model.Name = ProductName;
                model.MarketName = ProductMarketName;
                model.Price = ProductPrice;
                model.Id = 0;
                correctProductsCollection.Add(model);
                Session["CorrectProducts"] = correctProductsCollection;

                var dupCorrectProducts = (List<List<SellProductModel>>)Session["duplicateProducts"];
                string[] productNames = ProductName.Split(';');
                for (int h = 0; h < productNames.Count(); h++)
                {
                    var status = false;
                    if (productNames[h].ToString() != "")
                    {
                        for (int i = 0; i < dupCorrectProducts.Count; i++)
                        {
                            if (status == true)
                            {
                                break;
                            }
                            for (int j = 0; j < dupCorrectProducts[i].Count; j++)
                            {
                                var nameDupProduct = dupCorrectProducts[i][j].Name;
                                if (productNames[h].ToString() == nameDupProduct.ToString())
                                {
                                    dupCorrectProducts[i].Remove(dupCorrectProducts[i][j]);
                                    //if (dupCorrectProducts[i].Count == 0)
                                    //{
                                    //    dupCorrectProducts.Remove(dupCorrectProducts[i]);
                                    //}
                                    status = true;
                                    break;
                                }
                                Session["duplicateProducts"] = dupCorrectProducts;
                            }

                            if (dupCorrectProducts[i].Count == 0)
                            {
                                dupCorrectProducts.Remove(dupCorrectProducts[i]);
                            }
                        }
                    }
                }
                result.id = 0;
                result.correctProductName = ProductName;
                result.correctMarketName = ProductMarketName;
                result.correctProductPrice = ProductPrice;
            }
            return Json(result);
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

        public JsonResult exportTxt()
        {
            var assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var directoryPath = Path.GetDirectoryName(assemblyPath);
            var text = Path.GetDirectoryName(directoryPath);
            var filePath = Path.Combine(text, "UploadedExcelFiles\\ProductName.txt");

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Close();

                TextWriter sw = new StreamWriter(text + "\\UploadedExcelFiles\\ProductName.txt");
                var correctDupProducts = (List<List<SellProductModel>>)Session["duplicateProducts"];

                for (int i = 0; i < correctDupProducts.Count; i++)
                {
                    var productName = "";
                    //        sw.WriteLine(dataGridView1.Rows[i].Cells[0].Value.ToString() + "\t" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "\t" + dataGridView1.Rows[i].Cells[2].Value.ToString());
                    for (int j = 0; j < correctDupProducts[i].Count; j++)
                    {

                        productName += correctDupProducts[i][j].Name + ";";
                    }
                    sw.WriteLine(productName + "\t");
                }
                sw.Close();
            }
            var message = "Lưu file thành công";
            return Json(message);
        }
    }
}
