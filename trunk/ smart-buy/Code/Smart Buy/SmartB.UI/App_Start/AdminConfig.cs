using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SmartB.UI
{
    public static class AdminConfig
    {
        public static void Register(HttpServerUtility server)
        {
            string logPath = server.MapPath("~/Areas/Admin/LogFiles/");
            string xmlPath = server.MapPath("~/Areas/Admin/AdminConfig.xml");
            XDocument doc = XDocument.Load(xmlPath);
            var element = doc.Root.Element("logPath");
            if (element != null)
            {
                element.Value = logPath;
            }
            doc.Save(xmlPath);
        }
    }
}