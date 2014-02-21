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

        public ActionResult Statistics()
        {
            return View();
        }

    }
}
