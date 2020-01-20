using CellableMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CellableMVC.Controllers
{
    public class HomeController : Controller
    {
        private CellableEntities db = new CellableEntities();

        public ActionResult Index()
        {
            var title = db.SystemSettings.Find(18);
            var text = db.SystemSettings.Find(2);
            var footer = db.SystemSettings.Find(3);
            var Slide1 = db.SystemSettings.Find(4);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;
            ViewBag.Slide1 = Slide1.Value;

            var phones = db.Phones.ToList();

            return View(phones);
        }

        public ActionResult About()
        {
            var title = db.SystemSettings.Find(39);
            var text = db.SystemSettings.Find(40);
            var footer = db.SystemSettings.Find(41);
            var image = db.SystemSettings.Find(42);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;
            ViewBag.Image = image.Value;

            return View();
        }

        public ActionResult Contact()
        {
            var title = db.SystemSettings.Find(39);
            var address = db.SystemSettings.Find(9);
            var city = db.SystemSettings.Find(11);
            var state = db.SystemSettings.Find(12);
            var zip = db.SystemSettings.Find(13);
            var phone = db.SystemSettings.Find(14);
            var contactEmail = db.SystemSettings.Find(7);
            var adminEmil = db.SystemSettings.Find(8);

            ViewBag.Title = title.Value;
            ViewBag.Address = address.Value;
            ViewBag.City = city.Value;
            ViewBag.State = state.Value;
            ViewBag.Zip = zip.Value;
            ViewBag.Phone = phone.Value;
            ViewBag.ContactEmail = contactEmail.Value;
            ViewBag.AdminEmail = adminEmil.Value;

            return View();
        }
    }
}