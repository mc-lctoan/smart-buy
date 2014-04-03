using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models.EntityFramework;
using PagedList;

namespace SmartB.UI.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "admin")]
    public class ManageUserController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();
        private const int PageSize = 10;
        
        public ActionResult Index(int page = 1)
        {
            var users = context.Users
                .Include(x => x.Role)
                .OrderBy(x => x.Username)
                .Where(x => x.IsActive && x.Role.Name != "admin").ToPagedList(page, PageSize);
            return View(users);
        }

        public ActionResult Create()
        {
            var roles = context.Roles.ToList();
            var roleList = new SelectList(roles, "Id", "Name");
            ViewBag.Roles = roleList;
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult Create(User model)
        {
            if (ModelState.IsValid)
            {
                model.IsActive = true;
                context.Users.Add(model);
                context.SaveChanges();
                TempData["create"] = "Success";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string username)
        {
            var user = context.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Username == username);
            var roles = context.Roles.ToList();
            var roleList = new SelectList(roles, "Id", "Name");
            ViewBag.Roles = roleList;
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User model)
        {
            var user = context.Users.FirstOrDefault(x => x.Username == model.Username);
            string message;
            if (user != null)
            {
                if (model.Password != null)
                {
                    user.Password = model.Password;
                }
                user.RoleId = model.RoleId;
                context.SaveChanges();
                message = "Success";
            }
            else
            {
                message = "Failed";
            }
            TempData["edit"] = message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToRouteResult Delete(string[] usernames)
        {
            foreach (string username in usernames)
            {
                var user = context.Users.FirstOrDefault(x => x.Username == username);
                if (user != null)
                {
                    user.IsActive = false;
                }
            }
            context.SaveChanges();
            TempData["delete"] = "Done";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult SetActive(string username)
        {
            var user = context.Users.FirstOrDefault(x => x.Username == username);
            bool statusFlag = false;
            if (ModelState.IsValid)
            {
                if (user.IsActive == true)
                {
                    user.IsActive = false;
                    statusFlag = false;
                }
                else
                {
                    user.IsActive = true;
                    statusFlag = true;
                }
                context.SaveChanges();
            }

            // Display the confirmation message
            var results = new User
            {
                IsActive = statusFlag
            };

            return Json(results);
        }
    }
}
