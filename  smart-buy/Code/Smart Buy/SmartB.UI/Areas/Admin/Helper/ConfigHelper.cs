using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using SmartB.UI.Infrastructure;

namespace SmartB.UI.Areas.Admin.Helper
{
    public class ConfigHelper
    {
        private readonly string _xmlPath = ConstantManager.ConfigPath;

        public string GetLogPath()
        {
            XDocument doc = XDocument.Load(_xmlPath);
            var element = doc.Root.Element("logPath");
            if (element != null)
            {
                return element.Value;
            }
            return "";
        }

        public void TurnParser(bool on)
        {
            XDocument doc = XDocument.Load(_xmlPath);
            var element = doc.Root.Element("IsParserRunning");
            if (element != null)
            {
                if (on)
                {
                    element.Value = 1 + "";
                }
                else
                {
                    element.Value = 0 + "";
                }
            }
        }

        public void SetFuel(int value)
        {
            XDocument doc = XDocument.Load(_xmlPath);
            var element = doc.Root.Element("fuel");
            if (element != null)
            {
                element.Value = value + "";
            }
        }

        public int GetFuel()
        {
            XDocument doc = XDocument.Load(_xmlPath);
            var element = doc.Root.Element("fuel");
            if (element != null)
            {
                int price;
                if (Int32.TryParse(element.Value, out price))
                {
                    return price;
                }
            }
            return -1;
        }
    }
}