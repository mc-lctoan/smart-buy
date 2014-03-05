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
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Areas.Admin.Controllers
{
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

            string[] lines = System.IO.File.ReadAllLines(Server.MapPath("~/UploadedExcelFiles/ProductName.txt"));
            List<List<DictionaryModel>> listDupDictionary = listDictionaries(lines);
            ViewBag.dupDictionary = listDupDictionary;
            Session["ListDictionaries"] = listDupDictionary;
            return View();
        }


        private static int? CheckProductNameWithDictionary(string productNameFirst, DbSet<Dictionary> dictionaries)
        {
            foreach (var dictionary in dictionaries.ToList())
            {
                double compare = CompareStringHelper.CompareString(dictionary.Name, productNameFirst);
                if (compare > 0.7)
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
        public JsonResult MergeProductTraining(String productJson, string productName)
        {
            var check = false;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<DictionaryModel> parseJson = ser.Deserialize<List<DictionaryModel>>(productJson);
            try
            {
                var pName = parseJson[parseJson.Count - 1].Name;
                var dupDictionary = db.Dictionaries.Where(d => d.Name == pName).FirstOrDefault();
             
                //check co chon item trong db hay ko
                if (dupDictionary != null)
                {
                    for (int i = 0; i < parseJson.Count; i++)
                    {
                        //var pId = parseJson[i].ProductId;
                        // var pName = parseJson[i].Name;
                        var pId = dupDictionary.ProductId;
                        var dupName = db.Dictionaries.Where(d => d.ProductId == pId);
                        if (!dupName.Any(n => n.Name.Equals(pName)))
                        {
                            var dictionary = new Dictionary();
                            dictionary.Name = pName;
                            dictionary.ProductId = pId;
                            db.Dictionaries.Add(dictionary);
                            db.SaveChanges();
                        }
                    }
                    //else
                    //{
                    //    var dupProductName = db.Products.Where(p => p.Name == pName).FirstOrDefault();
                    //    if (dupProductName == null)
                    //    {
                    //        var newProduct = new Product();
                    //        newProduct.Name = pName;
                    //        newProduct.IsActive = true;
                    //        db.Products.Add(newProduct);

                    //        var newDictionary = new Dictionary();
                    //        newDictionary.Name = pName;
                    //        newDictionary.Product = newProduct;
                    //        db.Dictionaries.Add(newDictionary);

                    //        db.SaveChanges();
                    //    }
                    //    else
                    //    {
                    //        var newDictionary = new Dictionary();
                    //        newDictionary.Name = pName;
                    //        newDictionary.ProductId = dupProductName.Id;
                    //        db.Dictionaries.Add(newDictionary);
                    //    }
                    //}



                }
                else
                {
                    pName = parseJson[0].Name;
                    var dupProductName = db.Products.Where(p => p.Name == pName).FirstOrDefault();
                    dupDictionary = db.Dictionaries.Where(d => d.Name == pName).FirstOrDefault();
                    if (dupProductName == null)
                    {
                        var newProduct = new Product();
                        newProduct.Name = pName;
                        newProduct.IsActive = true;
                        db.Products.Add(newProduct);
                        if (dupDictionary == null)
                        {
                            var newDictionary = new Dictionary();
                            newDictionary.Name = pName;
                            newDictionary.Product = newProduct;
                            db.Dictionaries.Add(newDictionary);
                        }
                    }
                    else
                    {
                        if (dupDictionary == null)
                        {
                            var newDictionary = new Dictionary();
                            newDictionary.Name = pName;
                            newDictionary.ProductId = dupProductName.Id;
                            db.Dictionaries.Add(newDictionary);
                        }
                    }
                    db.SaveChanges();

                    var pId = db.Products.Where(p => p.Name == pName).FirstOrDefault().Id;
                    for (int i = 1; i < parseJson.Count; i++)
                    {
                        pName = parseJson[i].Name;
                        dupDictionary = db.Dictionaries.Where(d => d.Name == pName).FirstOrDefault();
                        if (dupDictionary == null)
                        {
                            var newDictionary = new Dictionary();
                            newDictionary.Name = parseJson[i].Name;
                            newDictionary.ProductId = pId;
                            db.Dictionaries.Add(newDictionary);
                            db.SaveChanges();
                        }
                    }
                }

                writeToTxt(productName);

                check = true;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
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
                    var dupDictionaryName = db.Dictionaries.Where(d => d.Name == pName).FirstOrDefault();

                    if (dupProductName == null)
                    {
                        var newProduct = new Product();
                        newProduct.Name = pName;
                        newProduct.IsActive = true;
                        db.Products.Add(newProduct);
                        if (dupDictionaryName == null)
                        {

                            var newDictionary = new Dictionary();
                            newDictionary.Name = pName;
                            newDictionary.Product = newProduct;
                            db.Dictionaries.Add(newDictionary);
                        }
                        db.SaveChanges();
                    }

                }

                writeToTxt(productName);

                check = true;
                return Json(check, JsonRequestBehavior.AllowGet);
            }
            catch
            {
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

            foreach (string line in lines)
            {
                int id = 0;
                List<DictionaryModel> dictionaries = new List<DictionaryModel>();
                List<DictionaryModel> dupDictionaries = new List<DictionaryModel>();
                // Use a tab to indent each line of the file.
                var productName = line.Split(';');
                for (int i = 0; i < productName.Length; i++)
                {
                    var name = productName[i];

                    var productId = CheckProductNameWithDictionary(name, db.Dictionaries);
                    if (productId != 0)
                    {
                        dupDictionaries.Add(new DictionaryModel
                        {
                            Name = name,
                            ProductId = productId.GetValueOrDefault()
                        });
                        id = productId.GetValueOrDefault();

                    }
                }



                if (dupDictionaries != null && dupDictionaries.Count > 0)
                {
                    listDupDictionary.Add(dupDictionaries);
                }
            }

            return listDupDictionary;
        }
    }
}
