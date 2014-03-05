using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace SmartB.UI.Models
{
    public class ListSellProductModel
    {
        public List<SellProductModel> CorrectSellProducts { get; set; }
        public List<SellProductModel> InCorrectSellProducts { get; set; }
        public IPagedList<SellProductModel> PagedCorrectProducts { get; set; }
    }
}