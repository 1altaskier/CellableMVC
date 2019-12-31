using CellableMVC.Models;
using System.Linq;
using System.Web.Mvc;

namespace CellableMVC.Controllers
{
    public class HomeController : Controller
    {
        private CellableEntities db = new CellableEntities();

        public ActionResult Index()
        {
            var phones = db.Phones.ToList();

            return View(phones);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}