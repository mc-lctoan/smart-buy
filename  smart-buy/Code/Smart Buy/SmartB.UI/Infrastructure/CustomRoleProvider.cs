using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SmartB.UI.Models.EntityFramework;
using WebMatrix.WebData;

namespace SmartB.UI.Infrastructure
{
    public class CustomRoleProvider : SimpleRoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            bool result = false;
            using (var context = new SmartBuyEntities())
            {
                var user = context.Users
                    .Include(x => x.Role)
                    .FirstOrDefault(x => x.Username == username);
                if (user != null)
                {
                    if (user.Role.Name == roleName)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public override string[] GetRolesForUser(string username)
        {
            var result = new string[1];
            using (var context = new SmartBuyEntities())
            {
                var user = context.Users
                    .Include(x => x.Role)
                    .FirstOrDefault(x => x.Username == username);
                if (user != null)
                {
                    result[0] = user.Role.Name;
                }
            }
            return result;
        }
    }
}