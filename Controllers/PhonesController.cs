using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

            Session["VersionName"] = phoneVersion.Version;
            Session["PhoneValue"] = phoneVersion.BaseCost;
            Session["ImageLocation"] = phoneVersion.ImageName;
            Session["VersionId"] = phoneVersion.VersionId;

            return View(possibleDefects);
        }

        public ActionResult PricePhone(FormCollection form)
        {
            PhoneVersion version = db.PhoneVersions.Find(Session["VersionId"]);

            var baseCost = version.BaseCost;

            foreach (var item in form)
            {
                if (item.ToString() != "__RequestVerificationToken" &&
                    item.ToString() != "id" &&
                    item.ToString() != "capacity")
                {
                    baseCost -= decimal.Parse(Request.Form[item.ToString()]);
                    Session[item.ToString()] = Request.Form[item.ToString()];
                }
            }

            return View();
        }

        public ActionResult CalcPromo(string PromoCode)
        {
            try
            {
                Promo promo = db.Promos.FirstOrDefault(x => x.PromoCode == PromoCode && (x.StartDate < DateTime.Today && x.EndDate > DateTime.Today));

                var promoDiscount = decimal.Parse(Session["PhoneValue"].ToString()) * promo.Discount;
                Session["PromoCode"] = PromoCode;
                Session["PromoDiscount"] = promo.Discount;
                Session["PhoneValue"] = decimal.Parse(Session["PhoneValue"].ToString()) - promoDiscount;
            }
            catch(Exception ex)
            {

            }

            return RedirectToAction("PricePhone", "Phones");
        }
    }
}