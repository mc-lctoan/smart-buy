using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Areas.Admin.Helper;
using SmartB.UI.Areas.Admin.Models;

namespace SmartB.UI.Areas.Admin.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {
        //
        // GET: /Admin/System/

        public RedirectToRouteResult Processing()
        {
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "ManageUser");
            }
            if (User.IsInRole("staff"))
            {
                return RedirectToAction("TrainingMatch", "Training");
            }
            if (User.IsInRole("member"))
            {
                return RedirectToAction("Index", "Home");
            }
            return null;
        }

        public ActionResult ConfigureSystem()
        {
            var helper = new ConfigHelper();
            var model = helper.CreateModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateSystemProperties(ConfigurationModel model)
        {
            var helper = new ConfigHelper();
            helper.UpdateModel(model);
            TempData["edit"] = "Success";
            return RedirectToAction("ConfigureSystem");
        }
    }
}
