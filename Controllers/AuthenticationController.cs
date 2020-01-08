using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CellableMVC.Models;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using CellableMVC.Helpers;

namespace EF_CRUD.Controllers
{
    public class AuthenticationController : Controller
    {
        private CellableEntities db = new CellableEntities();
        private LoggedInUser loggedInUser = new LoggedInUser();

        [ValidateAntiForgeryToken]
        public ActionResult UserLogin([Bind(Include = "userName,password,rememberMe")]string userName, string password)
        {
            User user = new User();

            try
            {
                user = db.Users.Single(x => x.UserName == userName && x.Password == password);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Users", new { msg = "Incorrect User Name or Password" });
            }

            int temp = 0;
            if (int.TryParse(user.UserId.ToString(), out temp))
            {
                Session["LoggedInUserId"] = user.UserId;
                Session["LoggedInUser"] = user.UserName;
                loggedInUser.UserId = user.UserId;
                loggedInUser.UserName = user.UserName;


                // Update Last Login Date & Time
                var updateUser = new User() { UserId = int.Parse(loggedInUser.UserId.ToString()), LastLogin = DateTime.Now };
                using (var db = new CellableEntities())
                {
                    db.Users.Attach(updateUser);
                    db.Entry(updateUser).Property(x => x.LastLogin).IsModified = true;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                }

                var rememberMe = false;
                // Check if the user wants to be remembered
                if (rememberMe)
                {
                    //cookie["UserName"] = "";
                    //cookie.Expires = DateTime.Now.AddDays(90);
                    //Response.Cookies.Add(cookie);
                }
                else
                {
                    // Set the cookie as normal
                    FormsAuthentication.SetAuthCookie(userName, false);
                }

                return RedirectToAction("TrackOrders", "Users");
            }
            else
            {
                return RedirectToAction("Login", "Users", new { msg = "Incorrect User Name or Password" });
            }
        }

        // User Logout
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Users");
        }
    }
}