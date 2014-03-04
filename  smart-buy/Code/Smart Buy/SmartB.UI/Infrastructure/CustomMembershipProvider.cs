using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SmartB.UI.Models.EntityFramework;
using WebMatrix.WebData;

namespace SmartB.UI.Infrastructure
{
    public class CustomMembershipProvider : SimpleMembershipProvider
    {
        public override bool ValidateUser(string username, string password)
        {
            using (var context = new SmartBuyEntities())
            {
                var user = context.Users.FirstOrDefault(x => x.Username == username && x.Password == password);
                if (user != null)
                {
                    return true;
                }
                return false;
            }
        }
    }
}