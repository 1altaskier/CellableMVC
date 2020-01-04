using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CellableMVC.Models;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace EF_CRUD.Controllers
{
    public class AuthenticationController : Controller
    {
        private CellableEntities db = new CellableEntities();

        [ValidateAntiForgeryToken]
        public ActionResult UserLogin([Bind(Include = "userName,password,rememberMe")]string userName, string password)
        {
            IList<User> users = null;

            users = db.Users.ToList().Where(x => x.UserName == userName && x.Password == password).ToList();

            if (users.Count > 0)
            {
                Session["LoggedInUser"] = userName;

                var rememberMe = false;
                // Check if the user wants to be remembered
                if (rememberMe)
                {
                    //var cookie = FormsAuthentication.GetAuthCookie(userName, rememberMe);
                    //var salt = Hmac.GenerateSalt();
                    //var hashPass = Hmac.ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(password), salt);
                    //cookie["Password"] = hashPass;
                    //cookie.Expires = DateTime.Now.AddDays(30);
                    //Response.Cookies.Add(cookie);
                }
                else
                {
                    // Set the cookie as normal
                    FormsAuthentication.SetAuthCookie(userName, false);
                }

                return RedirectToAction("TrackProgress", "Users");
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

    public static class Hmac
    {
        private const int SaltSize = 32;

        public static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[SaltSize];

                rng.GetBytes(randomNumber);

                return randomNumber;

            }
        }

        public static byte[] ComputeHMAC_SHA256(byte[] data, byte[] salt)
        {
            using (var hmac = new HMACSHA256(salt))
            {
                return hmac.ComputeHash(data);
            }
        }
    }
}