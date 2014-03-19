using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartB.UI.Infrastructure
{
    public static class ConstantManager
    {
        public static string LogPath { get; set; }
        public static string ConfigPath { get; set; }
    }

    public enum SystemRole
    {
        Admin = 1,
        Staff = 2,
        Member = 3
    }
}