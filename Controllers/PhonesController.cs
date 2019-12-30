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

        public ActionResult DefectQuestions(int? id)
        {
            // Initialize Defects Variable
            IList<PossibleDefect> possibleDefects = null;

            // Initialize Defect Groups
            IList<DefectGroup> defectGroup = db.DefectGroups.ToList();


            // Get a list of Defects to pass to the view
            possibleDefects = db.PossibleDefects.ToList().OrderBy(x => x.DefectGroup.DisplayOrder).Where(x => x.VersionId == id).ToList();

            PhoneVersion phoneVersion = db.PhoneVersions.Find(id);

            // Get Phone Image to display
            ViewBag.ImageLocation = phoneVersion.ImageName;
            ViewBag.VersionName = phoneVersion.Version;

            ViewBag.ID = phoneVersion.PhoneId;

            return View(possibleDefects);
        }

        public ActionResult PricePhone(int? id, FormCollection form)
        {
            PhoneVersion version = db.PhoneVersions.Find(id);

            var baseCost = version.BaseCost;

            return View();
        }
    }
}