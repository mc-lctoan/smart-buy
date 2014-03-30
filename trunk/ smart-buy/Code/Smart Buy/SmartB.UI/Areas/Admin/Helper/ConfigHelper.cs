using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using FluentScheduler;
using SmartB.UI.Areas.Admin.Models;
using SmartB.UI.Infrastructure;

namespace SmartB.UI.Areas.Admin.Helper
{
    public class ConfigHelper
    {
        private readonly string _xmlPath = ConstantManager.ConfigPath;

        public ConfigurationModel CreateModel()
        {
            var model = new ConfigurationModel
                            {
                                FuelPrice = GetFuel(), 
                                Epsilon = GetEpsilon(), 
                                ParseTime = GetParseTime()
                            };
            return model;
        }

        public void UpdateModel(ConfigurationModel model)
        {
            SetFuel(model.FuelPrice);
            SetEpsilon(model.Epsilon);
            SetParseTime(model.ParseTime);
        }

        private int GetEpsilon()
        {
            XDocument doc = XDocument.Load(_xmlPath);
            var element = doc.Root.Element("epsilon");
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

        private void SetEpsilon(int epsilon)
        {
            XDocument doc = XDocument.Load(_xmlPath);
            var element = doc.Root.Element("epsilon");
            if (element != null)
            {
                element.Value = epsilon + "";
            }
            doc.Save(_xmlPath);
        }

        private string GetParseTime()
        {
            string result = "";
            XDocument doc = XDocument.Load(_xmlPath);
            var hour = doc.Root.Element("parseTime").Element("hour");
            if (hour != null)
            {
                result += hour.Value + ":";
            }
            var minute = doc.Root.Element("parseTime").Element("minute");
            if (minute != null)
            {
                result += minute.Value;
            }
            return result;
        }

        private void SetParseTime(string time)
        {
            if (!String.IsNullOrEmpty(time))
            {
                string[] tmp = time.Split(':');
                string hour = tmp[0];
                string minute = tmp[1];

                XDocument doc = XDocument.Load(_xmlPath);
                var elementH = doc.Root.Element("parseTime").Element("hour");
                if (elementH != null)
                {
                    elementH.Value = hour;
                }
                var elementM = doc.Root.Element("parseTime").Element("minute");
                if (elementM != null)
                {
                    elementM.Value = minute;

                }
                doc.Save(_xmlPath);

                int h = Int32.Parse(hour);
                int m = Int32.Parse(minute);
                TaskManager.RemoveTask("Parser");
                TaskManager.Initialize(new ParseService(h, m));
            }
        }

        private void SetFuel(int value)
        {
            XDocument doc = XDocument.Load(_xmlPath);
            var element = doc.Root.Element("fuel");
            if (element != null)
            {
                element.Value = value + "";
            }
            doc.Save(_xmlPath);
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