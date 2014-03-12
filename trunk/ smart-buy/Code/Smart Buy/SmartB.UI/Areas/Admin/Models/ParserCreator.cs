using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartB.UI.Areas.Admin.Models
{
    public class ParserCreator
    {
        [Display(Name = "Link cần lấy")]
        public string ParseLink { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string ProductNameXpath { get; set; }

        [Display(Name = "Giá sản phẩm")]
        public string PriceXpath { get; set; }

        [Display(Name = "Phân trang")]
        public string PagingXpath { get; set; }

        public int MarketId { get; set; }
    }
}