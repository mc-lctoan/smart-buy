using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartB.UI.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/

        public ActionResult SearchProduct()
        {
            return View();
        }

        public ActionResult ViewCart()
        {
            return View();
        }
    }
}
