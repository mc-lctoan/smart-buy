﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SmartBuyEntities : DbContext
    {
        public SmartBuyEntities()
            : base("name=SmartBuyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<History> Histories { get; set; }
        public DbSet<LogFile> LogFiles { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SellProduct> SellProducts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPrice> UserPrices { get; set; }
        public DbSet<ParseInfo> ParseInfoes { get; set; }
    }
}
