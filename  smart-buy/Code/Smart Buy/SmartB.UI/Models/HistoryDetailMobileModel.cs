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
        public int Id { get; set; }
        public int HistoryId { get; set; }
        public string ProductName { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int MinPriceToday { get; set; }
        public int MaxPriceToday { get; set; }
    }
}