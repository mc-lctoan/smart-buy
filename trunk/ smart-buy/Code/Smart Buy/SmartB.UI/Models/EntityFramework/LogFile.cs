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
    
    public partial class LogFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public bool IsActive { get; set; }
    }
}
