using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Models.EntityFramework;
using System.Data.Entity;
using SmartB.UI.Models;
using SmartB.UI.UploadedExcelFiles;

namespace SmartB.UI.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        private SmartBuyEntities db = new SmartBuyEntities();
        public ActionResult SearchProduct(String q)
        {                     
            if (!String.IsNullOrEmpty(q))
            {                
                var products = db.ProductAttributes.Include(x => x.Product).Where(s => s.Product.Name.Contains(q));                
                return View(products);
            }

            else
            {
                return View();
            }
        }

        public ActionResult ViewCart()
        {
            return View();
        }

        public ActionResult UploadProduct()
        {
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

                //Catch exception 
                
                string errorName = "";
                string errorMarket = "";
                string errorPrice = "";
                int errorCount = 0; // Đếm tổng số lỗi
                try
                {
                    sellProductCollection = excelHelper.ReadData((Server.MapPath(savedFileName)), out errorName, out errorMarket, out errorPrice, out errorCount);
                    ViewBag.SellProductCollection = sellProductCollection;
                    ViewBag.ExceptionName = errorName;
                    ViewBag.ExceptionMarket = errorMarket;
                    ViewBag.ExceptionPrice = errorPrice;
                    ViewBag.errorCount = errorCount;
                }
                catch (Exception exception)
                {
                    ViewBag.Exception = exception.Message;
                    ViewBag.ExceptionName = exception.Message;
                    ViewBag.ExceptionMarket = exception.Message;
                    ViewBag.ExceptionPrice = errorPrice;
                    ViewBag.errorCount = errorCount;
                }
                return View(sellProductCollection);

            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveProducts(List<ExcelUtilities.SellProduct> model)
        {
            if (ModelState.IsValid)
            {
                foreach (var product in model)
                {
                    SmartBuyEntities db = new SmartBuyEntities();
                   //Trạng thái khi lưu xuống db
                   int countUpdate = 0;
                   int countInsert = 0;
                    //Trung db
                    var dupMarket = db.Markets.Where(m => m.Name.Equals(product.MarketName)).FirstOrDefault();
                    var dupProduct = db.Products.Where(p => p.Name.Equals(product.Name)).FirstOrDefault();

                    if (dupMarket != null & dupProduct != null)
                    {
                        var sellProduct = db.SellProducts.Where(s => s.MarketId == dupMarket.Id).FirstOrDefault();
                        {
                            if (product.Price > 0)
                            {
                                sellProduct.SellPrice = product.Price;
                            }
                        };
                        countUpdate++;
                        //if (product.Price > 0)
                        //{
                        //    var addedSellProduct = db.SellProducts.Add(sellProduct);
                        //}
                        db.SaveChanges(); // Save to database
                    }
                    else if (dupMarket != null & dupProduct == null)
                    {
                        var newProduct = new SmartB.UI.Models.EntityFramework.Product // add Product
                        {
                            Name = product.Name,
                            IsActive = true,
                        };
                        var addedProduct = db.Products.Add(newProduct);

                        var sellProduct = new SmartB.UI.Models.EntityFramework.SellProduct //add SellProduct
                        {
                            Market = dupMarket,
                            Product = newProduct,
                            SellPrice = product.Price,
                            LastUpdatedTime = DateTime.Now
                        };
                        var addedSellProduct = db.SellProducts.Add(sellProduct);
                        countInsert++;
                        db.SaveChanges(); // Save to database
                    }
                    else if (dupMarket == null & dupProduct != null)
                    {
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
                    }
                    else
                    {
                        var market = new Market
                        {
                            Name = product.MarketName,
                            IsActive = true,
                        };
                        var newMarket = db.Markets.Add(market); //add market

                        var newProduct = new SmartB.UI.Models.EntityFramework.Product
                        {
                            Name = product.Name,
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
                    }
                }
            }
            
            return RedirectToAction("UploadProduct");
        }
    }
}
