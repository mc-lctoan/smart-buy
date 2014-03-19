using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Models
{
    public class AccountDetailModel
    {
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu cũ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        public List<Market> Markets { get; set; }

        public string FirstStartAddress { get; set; }
        public string FirstEndAddress { get; set; }
        public string FirstRoute { get; set; }
        public string FirstMarkets { get; set; }
        public string FirstRouteName { get; set; }
        public string SecondStartAddress { get; set; }
        public string SecondEndAddress { get; set; }
        public string SecondRoute { get; set; }
        public string SecondMarkets { get; set; }
        public string SecondRouteName { get; set; }
        public string ThirdStartAddress { get; set; }
        public string ThirdEndAddress { get; set; }
        public string ThirdRoute { get; set; }
        public string ThirdMarkets { get; set; }
        public string ThirdRouteName { get; set; }
    }
}