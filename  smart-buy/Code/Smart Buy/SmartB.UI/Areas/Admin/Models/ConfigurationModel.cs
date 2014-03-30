using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Areas.Admin.Models
{
    public class ConfigurationModel
    {
        public int FuelPrice { get; set; }
        public int Epsilon { get; set; }
        public string ParseTime { get; set; }
    }
}