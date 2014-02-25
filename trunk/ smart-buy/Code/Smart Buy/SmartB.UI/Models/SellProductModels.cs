using SmartB.UI.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartB.UI.Models
{
    public class SellProductModel
    {
         public int Id { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm phải từ 5 đến 20 ký tự."
            , MinimumLength = 5)]
        public string Name { get; set; }
        [MaxLength(20)]
        [Required(ErrorMessage = "Vui lòng nhập tên chợ")]
        [StringLength(20, ErrorMessage = "Tên chợ phải từ 5 đến 20 ký tự."
            , MinimumLength = 5)]
        public string MarketName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(1, 10000, ErrorMessage = "Vui lòng nhập giá trị từ 1 đến 10000")]
        [DisplayFormat(DataFormatString = "{0:0,000}")]
        public int Price { get; set; }
        public int RowNumber { get; set; }

        public static SellProductModel MapToSellProductEntity(SellProduct sellProduct) {
            SellProductModel model = new SellProductModel();
            model.Id = sellProduct.Id;
            model.MarketName = sellProduct.Market.Name;
            model.Name = sellProduct.Product.Name;
            model.Price = (int)sellProduct.SellPrice;
            return model;
        }
    }
}