using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentScheduler;
using FluentScheduler.Model;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models.EntityFramework;
using PagedList;

namespace SmartB.UI.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "staff")]
    public class LogFileController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();
        private const int PageSize = 10;

        public ActionResult Index(int page = 1)
        {
            var files = context.LogFiles
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedTime)
                .ToPagedList(page, PageSize);
            return View(files);
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
