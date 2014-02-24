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
    
    public partial class User
    {
        public User()
        {
            this.Histories = new HashSet<History>();
            this.UserPrices = new HashSet<UserPrice>();
        }
    
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DefinedRoute { get; set; }
        public string MarketId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    
        public virtual ICollection<History> Histories { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<UserPrice> UserPrices { get; set; }
    }
}
