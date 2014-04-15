using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SmartB.UI.Areas.Admin.Helper;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "staff")]
    public class TrainingController : Controller
    {
        //
        // GET: /Admin/Training/
        SmartBuyEntities db = new SmartBuyEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrainingMatch()
        {
            string path = Server.MapPath("~/UploadedExcelFiles/ProductName.txt");
            
            if (System.IO.File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                List<List<DictionaryModel>> listDupDictionary = listDictionaries(lines);
                Session["ListDictionaries"] = listDupDictionary;                
            }
            return View();
        }


        private static int? CheckProductNameWithDictionary(string productNameFirst, DbSet<Dictionary> dictionaries)
        {
            foreach (var dictionary in dictionaries.ToList())
            {
                double compare = CompareStringHelper.CompareString(dictionary.Name, productNameFirst);
                if (compare > 0)
                {
                    return dictionary.ProductId;
                }
            }
            return 0;
        }

        private static List<DictionaryModel> addDictionary(int? productId, string name)
        {
            List<DictionaryModel> dictionaries = new List<DictionaryModel>();
            dictionaries.Add(new DictionaryModel
            {
                Name = name,
                ProductId = productId.GetValueOrDefault()
            });
            return dictionaries;
        }

        [HttpGet, ActionName("MergeProductTraining")]
        public JsonResult MergeProductTraining(String productJson, string productName, int productId, int position)
        {
            var check = false;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<DictionaryModel> parseJson = ser.Deserialize<List<DictionaryModel>>(productJson);
            try
            {
                //var pName = parseJson[parseJson.Count - 1].Name;
                //var dupDictionary = db.Dictionaries.Where(d => d.Name == pName).FirstOrDefault();

                //pName = parseJson[0].Name;
                //var dupProductName = db.Products.Where(p => p.Name == pName).FirstOrDefault();
                //if (dupProductName == null)
                //{
                //    var newProduct = new Product();
                //    newProduct.Name = pName;
                //    newProduct.IsActive = true;
                //    db.Products.Add(newProduct);
                //    dupDictionary = db.Dictionaries.Where(d => d.Name == pName && d.ProductId == newProduct.Id).FirstOrDefault();

                //    if (dupDictionary == null)
                //    {
                //        var newDictionary = new Dictionary();
                //        newDictionary.Name = pName;
                //        newDictionary.Product = newProduct;
                //        db.Dictionaries.Add(newDictionary);
                //    }
                //}
                //else
                //{
                //    dupDictionary = db.Dictionaries.Where(d => d.Name == pName && d.ProductId == dupProductName.Id).FirstOrDefault();

                //    if (dupDictionary == null)
                //    {
                //        var newDictionary = new Dictionary();
                //        newDictionary.Name = pName;
                //        newDictionary.ProductId = dupProductName.Id;
                //        db.Dictionaries.Add(newDictionary);
                //    }
                //}
                //db.SaveChanges();

                //var pId = db.Products.Where(p => p.Name == pName).FirstOrDefault().Id;
                //for (int i = 1; i < parseJson.Count; i++)
                //{
                //    pName = parseJson[i].Name;
                //    dupDictionary = db.Dictionaries.Where(d => d.Name == pName).FirstOrDefault();
                //    if (dupDictionary == null)
                //    {
                //        var newDictionary = new Dictionary();
                //        newDictionary.Name = parseJson[i].Name;
                //        newDictionary.ProductId = pId;
                //        db.Dictionaries.Add(newDictionary);
                //        db.SaveChanges();
                //    }
                //}

                if (productId != 0)
                {
                    for (int i = 0; i < parseJson.Count; i++)
                    {
                        string pName = parseJson[i].Name;
                        var dupDictionary = db.Dictionaries.Where(d => d.Name == pName).FirstOrDefault();
                        if (dupDictionary == null)
                        {
                            var newDictionary = new Dictionary();
                            newDictionary.Name = parseJson[i].Name;
                            newDictionary.ProductId = productId;
                            db.Dictionaries.Add(newDictionary);                            
                        }
                        else
                        {
                            dupDictionary.ProductId = productId;                            
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    var proName = parseJson[position].Name;
                    var checkDupPName = db.Products.Where(p => p.Name == proName).FirstOrDefault();
                    var pId = 0;
                    var product = new Product();
                    if (checkDupPName == null)
                    {
                        var newProduct = new Product();
                        newProduct.Name = proName;
                        newProduct.IsActive = true;
                        db.Products.Add(newProduct);
                        pId = newProduct.Id;
                        product = newProduct;
                    }
                    else
                    {
                        pId = checkDupPName.Id;
                        product = checkDupPName;
                    }
                        var dictionary = db.Dictionaries.Where(d => d.Name == proName && d.ProductId == pId).FirstOrDefault();

                        if (dictionary == null)
                        {
                            var newDictionary = new Dictionary();
                            newDictionary.Name = proName;
                            newDictionary.Product = product;
                            db.Dictionaries.Add(newDictionary);
                        }
                        else
                        {
                            dictionary.ProductId = pId;
                        }
                        for (int i = 0; i < parseJson.Count; i++)
                        {
                            if (i != position)
                            {
                                string pName = parseJson[i].Name;
                                var dupDictionary = db.Dictionaries.Where(d => d.Name == pName).FirstOrDefault();
                                if (dupDictionary == null)
                                {
                                    var newDictionary = new Dictionary();
                                    newDictionary.Name = parseJson[i].Name;
                                    newDictionary.ProductId = pId;
                                    db.Dictionaries.Add(newDictionary);
                                }
                                else
                                {
                                    dupDictionary.ProductId = pId;
                                }
                            }
                            else { continue; }
                            //db.SaveChanges();
                        }
                    
                    db.SaveChanges();
                }

                writeToTxt(productName);
                TempData["MergeProduct"] = "Success";
                check = true;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                TempData["MergeProduct"] = "Failed";
                return Json(check, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet, ActionName("SplitProductTraining")]
        public JsonResult SplitProductTraining(String productJson, string productName)
        {
            var check = false;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<DictionaryModel> parseJson = ser.Deserialize<List<DictionaryModel>>(productJson);
            try
            {
                for (int i = 0; i < parseJson.Count; i++)
                {
                    var pName = parseJson[i].Name;
                    var dupProductName = db.Products.Where(p => p.Name == pName).FirstOrDefault();
                    var pId = 0;
                    var product = new Product();

                    if (dupProductName == null)
                    {
                        var newProduct = new Product();
                        newProduct.Name = pName;
                        newProduct.IsActive = true;
                        db.Products.Add(newProduct);
                        pId = newProduct.Id;
                        product = newProduct;
                    }
                    else
                    {
                        pId = dupProductName.Id;
                        product = dupProductName;
                    }
                        var dupDictionaryName = db.Dictionaries.Where(d => d.Name == pName && d.ProductId == pId).FirstOrDefault();
                        if (dupDictionaryName == null)
                        {

                            var newDictionary = new Dictionary();
                            newDictionary.Name = pName;
                            newDictionary.Product = product;
                            db.Dictionaries.Add(newDictionary);
                        }
                        db.SaveChanges();                    
                }

                writeToTxt(productName);
                TempData["SplitProduct"] = "Success";
                check = true;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                TempData["SplitProduct"] = "Failed";
                return Json(check, JsonRequestBehavior.AllowGet);
            }
        }

        public void writeToTxt(string productName)
        {
            // Xoa session 
            var dupDictionary = (List<List<DictionaryModel>>)Session["ListDictionaries"];            
            string[] productNames = productName.Split(';');
            for (int h = 0; h < productNames.Count(); h++)
            {
                var status = false;
                if (productNames[h].ToString() != "")
                {
                    for (int i = 0; i < dupDictionary.Count; i++)
                    {
                        if (status == true)
                        {
                            break;
                        }
                        for (int j = 0; j < dupDictionary[i].Count; j++)
                        {
                            var nameDupProduct = dupDictionary[i][j].Name;
                            if (productNames[h].ToString() == nameDupProduct.ToString())
                            {
                                dupDictionary[i].Remove(dupDictionary[i][j]);
                                status = true;
                                break;
                            }
                            Session["ListDictionaries"] = dupDictionary;
                        }

                        if (dupDictionary[i].Count == 0)
                        {
                            dupDictionary.Remove(dupDictionary[i]);
                        }
                    }

                }
            }
            //write to file txt
            var assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var directoryPath = Path.GetDirectoryName(assemblyPath);
            var text = Path.GetDirectoryName(directoryPath);
            var filePath = Path.Combine(text, "UploadedExcelFiles\\ProductName.txt");
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Close();

                TextWriter sw = new StreamWriter(text + "\\UploadedExcelFiles\\ProductName.txt");
                var correctDupProducts = (List<List<DictionaryModel>>)Session["ListDictionaries"];


                for (int i = 0; i < correctDupProducts.Count; i++)
                {
                    var proName = "";
                    for (int j = 0; j < correctDupProducts[i].Count; j++)
                    {
                        proName += correctDupProducts[i][j].Name + ";";
                    }
                    sw.WriteLine(proName + "\t");
                }
                sw.Close();
            }
        }


        public List<List<DictionaryModel>> listDictionaries(string[] lines)
        {
            List<List<DictionaryModel>> listDupDictionary = new List<List<DictionaryModel>>();
            //List<List<DictionaryModel>> listProductInDB = new List<List<DictionaryModel>>();

            foreach (string line in lines)
            {
                //List<DictionaryModel> products = new List<DictionaryModel>();
                List<DictionaryModel> dupDictionaries = new List<DictionaryModel>();
                // Use a tab to indent each line of the file.
                var productName = line.Split(';');
                for (int i = 0; i < productName.Length; i++)
                {
                    var name = productName[i];
                    var checkproductId = CheckProductNameWithDictionary(name, db.Dictionaries);
                    var product = db.Dictionaries.Where(p => p.Name.Equals(name)).Select(p => p.ProductId);
                    int productId = 0;
                    if (product != null)
                    {
                        productId = product.FirstOrDefault().GetValueOrDefault();
                    }

                    if (checkproductId != 0)
                    {
                        dupDictionaries.Add(new DictionaryModel
                        {
                            Name = name,
                            ProductId = productId
                        });

                        //var productInDB = db.Products.Where(p => p.Id == productId).FirstOrDefault();
                        //if (!products.Any(p => p.Name == productInDB.Name))
                        //{
                        //    products.Add(new DictionaryModel
                        //    {
                        //        Name = productInDB.Name,
                        //        ProductId = productInDB.Id
                        //    });
                        //}

                    }
                }

                if (dupDictionaries != null && dupDictionaries.Count > 0)
                {
                    listDupDictionary.Add(dupDictionaries);
                }

                //if (products != null && products.Count > 0)
                //{
                //    listProductInDB.Add(products);
                //}
            }
            ViewBag.dupDictionary = listDupDictionary;
            //ViewBag.dupProduct = listProductInDB;
            
            return listDupDictionary;
        }
    }
}
