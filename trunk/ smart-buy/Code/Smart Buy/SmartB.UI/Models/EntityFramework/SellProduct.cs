//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmartB.UI.Models.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class SellProduct
    {
        public int Id { get; set; }
        public Nullable<int> MarketId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> SellPrice { get; set; }
        public System.DateTime LastUpdatedTime { get; set; }
    
        public virtual Market Market { get; set; }
        public virtual Product Product { get; set; }
    }
}
