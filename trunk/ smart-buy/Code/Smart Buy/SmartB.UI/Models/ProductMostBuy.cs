using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Models
{
    public class ProductMostBuy
    {
        public Product Product { get; set; }
        public int numberOfBuy { get; set; }
    }
}