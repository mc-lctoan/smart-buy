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
        public string ExceptionName { get; set; }
        public string ExceptionMarket { get; set; }
        public string ExceptionPrice { get; set; }
        public int ErrorCount { get; set; }
        public List<string> ErrorNameLines { get; set; }
        public List<string> ErrorMarketNameLines { get; set; }
        public List<string> ErrorPriceLines { get; set; }
        public string Exception { get; set; }
        public List<List<SellProductModel>> duplicateCorrectProduct { get; set; }
        public int duplicateCorrectProductCount { get; set; }


    }
}