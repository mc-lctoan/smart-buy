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
    }
}