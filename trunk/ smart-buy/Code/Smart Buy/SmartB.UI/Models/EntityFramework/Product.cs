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
    
    public partial class Product
    {
        public Product()
        {
            this.Histories = new HashSet<History>();
            this.ProductAttributes = new HashSet<ProductAttribute>();
            this.SellProducts = new HashSet<SellProduct>();
            this.UserPrices = new HashSet<UserPrice>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }
        public virtual ICollection<SellProduct> SellProducts { get; set; }
        public virtual ICollection<UserPrice> UserPrices { get; set; }
    }
}
