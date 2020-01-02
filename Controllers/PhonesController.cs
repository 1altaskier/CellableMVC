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

        public ActionResult PhoneVersions(int? id)
        {
            // Initialize PhoneVersions Variable
            IList<PhoneVersion> phoneVersions = null;

            // Get a list of Phone Versions to pass to the view
            phoneVersions = db.PhoneVersions.ToList().Where(x => x.Phone.PhoneId == id).ToList();

            return View(phoneVersions);
        }

        public ActionResult DefectQuestions(int? id)
        {
            // Initialize Defects Variable
            IList<PossibleDefect> possibleDefects = null;
            
            // Get a list of Defects to pass to the view
            possibleDefects = db.PossibleDefects.ToList().OrderBy(x => x.DefectGroup.DisplayOrder).Where(x => x.VersionId == id).ToList();

            // Get the Version Info for this Particular Phone
            PhoneVersion phoneVersion = db.PhoneVersions.Find(id);

            Session["PhoneBrand"] = phoneVersion.PhoneId;
            Session["VersionName"] = phoneVersion.Version;
            Session["ImageLocation"] = phoneVersion.ImageName;
            Session["VersionId"] = phoneVersion.VersionId;
            Session["BaseCost"] = phoneVersion.BaseCost;

            // Get Phone Image to display
            ViewBag.ImageLocation = phoneVersion.ImageName;
            ViewBag.VersionName = phoneVersion.Version;

            // Get List of Storage Capacities
            IList<StorageCapacity> storage = db.StorageCapacities.ToList();
            ViewBag.Storage = storage;

            // Get List of Carriers
            IList<Carrier> carriers = db.Carriers.ToList();
            ViewBag.Carriers = carriers;

            return View(possibleDefects);
        }

        public ActionResult PricePhone(FormCollection form)
        {
            IList<PossibleDefect> defects = db.PossibleDefects.ToList();
            PhoneVersion version = db.PhoneVersions.Find(Session["VersionId"]);

            var baseCost = version.BaseCost;

            foreach (var item in form)
            {
                if (item.ToString() != "__RequestVerificationToken" &&
                    item.ToString() != "id" &&
                    item.ToString() != "capacity" && 
                    item.ToString() != "carriers")
                {
                    string defectField = Request.Form[item.ToString()];
                    string[] delimiter = new string[] { "_" };
                    string[] defect;
                    defect = defectField.Split(delimiter, StringSplitOptions.None);

                    if (defect[1] != "0" && defect[1] != "0.00")
                    {
                        baseCost -= decimal.Parse(defect[1]);
                        Session[defect[0]] = defect[1];
                    }
                }
            }

            if(!String.IsNullOrEmpty(Request.Form["capacity"]))
            {
                Session["Storage Capacity"] = Request.Form["capacity"];
            }

            StorageCapacity capacity  = db.StorageCapacities.Find(int.Parse(Session["Storage Capacity"].ToString()));
            // For Description on Pricing Page
            ViewBag.CapacityDesc = capacity.Description;

            if (!String.IsNullOrEmpty(Request.Form["carriers"]))
            {
                Session["Carrier"] = Request.Form["carriers"];
            }

            if (Session["Phone Value"] == null)
            {
                Session["Phone Value"] = baseCost;
            }

            return View(defects);
        }

        public ActionResult CalcPromo(string PromoCode)
        {
            if (!String.IsNullOrEmpty(PromoCode))
            {
                try
                {
                    Promo promo = db.Promos.FirstOrDefault(x => x.PromoCode == PromoCode && (x.StartDate < DateTime.Today && x.EndDate > DateTime.Today));

                    var promoDiscount = decimal.Parse(Session["Phone Value"].ToString()) * promo.Discount;
                    Session["PromoCode"] = PromoCode;
                    Session["PromoValue"] = promo.Discount;
                    Session["Phone Value"] = decimal.Parse(Session["Phone Value"].ToString()) + promoDiscount;
                }
                catch (Exception ex)
                {

                }
            }

            return RedirectToAction("PricePhone", "Phones");
        }
    }
}