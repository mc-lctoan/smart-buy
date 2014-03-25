using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using SmartB.UI.Models.EntityFramework;
using SmartB.UI.Models;

namespace SmartB.UI.Controllers
{
    public class AccountApiController : ApiController
    {
        private SmartBuyEntities db = new SmartBuyEntities();

        [HttpGet]
        public bool Login(String username, String password)
        {
            //JavaScriptSerializer ser = new JavaScriptSerializer();
            //LoginModel parseJson = ser.Deserialize<LoginModel>(loginJson);

            try
            {               

                bool HaveUser = db.Users.Any(u => u.Username == username && u.Password == password);

                if (HaveUser)
                {
                   // FormsAuthentication.SetAuthCookie(username, false);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }
    }
}
