using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Models.EntityFramework;
using System.Data.Entity;

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
    }
}
