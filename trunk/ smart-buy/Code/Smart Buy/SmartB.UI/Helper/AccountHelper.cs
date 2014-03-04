using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Helper
{
    public class AccountHelper
    {
        public void CreateAccount(RegisterModel model)
        {
            using (var context = new SmartBuyEntities())
            {
                // Duplicate account
                var account = context.Users.FirstOrDefault(x => x.Username == model.UserName);
                if (account != null)
                {
                    throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);
                }

                var newUser = new User
                                  {
                                      Username = model.UserName,
                                      Password = model.Password,
                                      Email = model.Email,
                                      RoleId = model.RoleId,
                                      IsActive = true
                                  };
                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }
    }
}