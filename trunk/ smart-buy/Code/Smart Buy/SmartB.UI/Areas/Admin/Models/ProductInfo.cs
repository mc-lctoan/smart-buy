using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Areas.Admin.Models
{
    public class ProductInfo
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}