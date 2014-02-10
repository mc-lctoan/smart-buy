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
        [MaxLength(20)]
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(20, ErrorMessage = "Tên sản phẩm phải từ 5 đến 20 ký tự."
            , MinimumLength = 5)]
        public string Name { get; set; }
        [MaxLength(20)]
        [Required(ErrorMessage = "Vui lòng nhập tên chợ")]
        [StringLength(20, ErrorMessage = "Tên chợ phải từ 5 đến 20 ký tự."
            , MinimumLength = 5)]
        public string MarketName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm")]
        [Range(0, 9999999, ErrorMessage = "Vui lòng nhập giá trị từ 1 đến 10000")]
        [DisplayFormat(DataFormatString = "{0:0,000}")]
        public int Price { get; set; } 
    }
}