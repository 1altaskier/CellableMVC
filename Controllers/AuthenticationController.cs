using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CellableMVC.Models;
using System.Web.Security;

namespace EF_CRUD.Controllers
{
    public class AuthenticationController : Controller
    {
        private CellableEntities db = new CellableEntities();

        [ValidateAntiForgeryToken]
        public ActionResult UserLogin([Bind(Include = "UserName,Password")]string userName, string password)
        {
            IList<User> users = null;

            users = db.Users.ToList().Where(x => x.UserName == userName && x.Password == password).ToList();

            if (users.Count > 0)
            {
                FormsAuthentication.SetAuthCookie(userName, false);
                return RedirectToAction("Dashboard", "Users");
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