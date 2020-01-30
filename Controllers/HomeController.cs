using CellableMVC.Models;
using CellableMVC.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CellableMVC.Helpers;

namespace CellableMVC.Controllers
{
    public class HomeController : Controller
    {
        private CellableEntities db = new CellableEntities();

        public ActionResult Index()
        {
            // Get the Average Star Rating
            //var avgRating = AvgRating();
            //General gen = new General();
            //gen.AvgStars = avgRating;

            var title = db.SystemSettings.Find(18);
            var text = db.SystemSettings.Find(2);
            var footer = db.SystemSettings.Find(3);
            var Slide1 = db.SystemSettings.Find(4);


            //ViewBag.AvgRating = avgRating;
            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;
            ViewBag.Slide1 = Slide1.Value;

            // Get Slide Show Images
            IList<SlideShow> slideShow = db.SlideShows.ToList();

            ViewBag.SlideShow = slideShow;

            var phones = db.Phones.ToList();

            return View(phones);
        }

        public ActionResult About()
        {
            //var avgRating = AvgRating();

            //vmAbout vmAbout = new vmAbout();
            //ViewBag.AvgRating = avgRating;

            // Get Testimonials
            IList<Testimonial> testimonial = db.Testimonials.ToList();

            //vmAbout.testimonials = testimonial;
            //vmAbout.title = db.SystemSettings.Find(39).ToString();
            //vmAbout.text = db.SystemSettings.Find(40).ToString();
            //vmAbout.body = db.SystemSettings.Find(49).ToString();
            //vmAbout.footer = db.SystemSettings.Find(41).ToString();
            //vmAbout.image = db.SystemSettings.Find(42).ToString();

            var title = db.SystemSettings.Find(39);
            var text = db.SystemSettings.Find(40);
            var body = db.SystemSettings.Find(49);
            var footer = db.SystemSettings.Find(41);
            var image = db.SystemSettings.Find(42);

            ViewBag.Testimonials = testimonial;
            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Body = body.Value;
            ViewBag.Footer = footer.Value;
            ViewBag.Image = image.Value;

            return View();
        }

        public ActionResult Contact()
        {
            //var avgRating = AvgRating();
            //ViewBag.AvgRating = avgRating;

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