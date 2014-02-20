using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Models
{
    public class HistoryDetailViewModel
    {
        public IEnumerable<ProductAttribute> ProductAttributes { get; set; }
        public History History { get; set; }

    }
}