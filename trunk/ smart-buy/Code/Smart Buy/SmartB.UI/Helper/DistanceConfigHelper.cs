using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models;

namespace SmartB.UI.Helper
{
    public static class DistanceConfigHelper
    {
        private static readonly string XmlPath = ConstantManager.DistanceFilePath;

        public static DistanceConfigModel GetData()
        {
            var model = new DistanceConfigModel();
            XDocument doc = XDocument.Load(XmlPath);

            var status = doc.Root.Element("status");
            if (status != null)
            {
                model.Status = Int32.Parse(status.Value);
            }

            var from = doc.Root.Element("from");
            if (from != null)
            {
                model.From = Int32.Parse(from.Value);
            }

            var to = doc.Root.Element("to");
            if (to != null)
            {
                model.To = Int32.Parse(to.Value);
            }

            return model;
        }

        public static void SetData(DistanceConfigModel model)
        {
            XDocument doc = XDocument.Load(XmlPath);

            var status = doc.Root.Element("status");
            if (status != null)
            {
                status.Value = model.Status + "";
            }

            var from = doc.Root.Element("from");
            if (from != null)
            {
                from.Value = model.From + "";
            }

            var to = doc.Root.Element("to");
            if (to != null)
            {
                to.Value = model.To + "";
            }

            doc.Save(XmlPath);
        }

        public static void Finish()
        {
            XDocument doc = XDocument.Load(XmlPath);

            var status = doc.Root.Element("status");
            if (status != null)
            {
                int tmp = (int) DistanceStatus.Finish;
                status.Value = tmp + "";
                doc.Save(XmlPath);
            }
        }
    }
}