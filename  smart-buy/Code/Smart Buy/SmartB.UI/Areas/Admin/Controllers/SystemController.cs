using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}
