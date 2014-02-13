using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Areas.Admin.Controllers
{
    public class LogFileController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();

        public ActionResult Index()
        {
            var files = context.LogFiles.Where(x => x.IsActive).ToList();
            return View(files);
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
