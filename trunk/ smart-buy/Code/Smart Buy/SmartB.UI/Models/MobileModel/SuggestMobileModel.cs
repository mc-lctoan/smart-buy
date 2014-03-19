using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Models.MobileModel
{
    public class SuggestMobileModel
    {
        public string Username { get; set; }
        public List<int> ProductIds { get; set; }
        public string RouteName { get; set; }
    }
}