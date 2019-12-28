using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CellableMVC.Controllers
{
    public class PhonesController : Controller
    {
        private CellableEntities db = new CellableEntities();

        // GET: Phones
        public ActionResult Phones()
        {
            var phones = db.Phones.ToList();

            return View(phones);
        }

        public ActionResult PhoneVersions(int? brandId)
        {
            // Initialize PhoneVersions Variable
            IList<PhoneVersion> phoneVersions = null;

            // Get a list of Phone Versions to pass to the view
            phoneVersions = db.PhoneVersions.ToList().Where(x => x.Phone.PhoneId == brandId).ToList();

            return View(phoneVersions);
        }
    }
}