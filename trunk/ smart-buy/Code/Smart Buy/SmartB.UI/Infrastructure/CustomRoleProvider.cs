using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace SmartB.UI.Infrastructure
{
    public class CustomRoleProvider : SimpleRoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            return base.IsUserInRole(username, roleName);
        }

        public override string[] GetRolesForUser(string username)
        {
            return base.GetRolesForUser(username);
        }
    }
}