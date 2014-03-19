using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentScheduler;
using SmartB.UI.Areas.Admin.Helper;
using SmartB.UI.Binder;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models;

namespace SmartB.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            AdminConfig.Register(Server);

            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());

            ConstantManager.LogPath = Server.MapPath("~/Areas/Admin/LogFiles/");
            ConstantManager.ConfigPath = Server.MapPath("~/Areas/Admin/AdminConfig.xml");
            MarketHelper.CalculateDistance();

            TaskManager.Initialize(new ParseService());
        }
    }
}