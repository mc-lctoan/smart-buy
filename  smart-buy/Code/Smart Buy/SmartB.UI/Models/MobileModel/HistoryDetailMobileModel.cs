using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartB.UI.Areas.Admin.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Models
{
    public class HistoryDetailMobileModel
    {
        public string Id { get; set; }
        public string HistoryId { get; set; }
        public string ProductName { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string MinPriceToday { get; set; }
        public string MaxPriceToday { get; set; }
    }
}