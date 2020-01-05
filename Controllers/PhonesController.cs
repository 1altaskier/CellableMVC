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

        public ActionResult Carriers(int? id)
        {
            Phone phoneBrand = db.Phones.Find(id);
            Session["BrandId"] = id;
            Session["BrandName"] = phoneBrand.Brand;

            IList<Carrier> carriers = db.Carriers.ToList();

            return View(carriers);
        }

        public ActionResult PhoneVersions(int? brandId, int? carrierId, string searchString = null)
        {
            // Initialize PhoneVersions Variable
            IList<PhoneVersion> phoneVersions = null;

            if (searchString == null)
            {
                // Get entire list of Phone Versions to pass to the view
                phoneVersions = db.PhoneVersions.ToList().Where(x => x.Phone.PhoneId == brandId).ToList();
            }
            else
            {
                // Get filtered Phone Versions list
                phoneVersions = db.PhoneVersions.Where(x => x.Version.Contains(searchString)).ToList();
            }


            // Set the Carrier Session Variable
            if (carrierId != null)
            {
                Session["Carrier"] = carrierId;
            }

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

            // Get Phone Brand to display
            Phone phoneBrand = db.Phones.Find(int.Parse(Session["PhoneBrand"].ToString()));
            Session["PhoneBrandName"] = phoneBrand.Brand;

            // Get Phone Image to display
            ViewBag.ImageLocation = phoneVersion.ImageName;
            ViewBag.VersionName = phoneVersion.Version;

            // Get List of Storage Capacities
            IList<StorageCapacity> storage = db.StorageCapacities.ToList();
            ViewBag.Storage = storage;

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
                    item.ToString() != "carriers" &&
                    item.ToString() != "hdCarrier" &&
                    item.ToString() != "hdCapacity")
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

            if (!String.IsNullOrEmpty(Request.Form["capacity"]))
            {
                Session["Storage Capacity"] = Request.Form["capacity"];
            }

            StorageCapacity capacity = db.StorageCapacities.Find(int.Parse(Session["Storage Capacity"].ToString()));
            // For Description on Pricing Page
            ViewBag.CapacityDesc = capacity.Description;

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

        public ActionResult Search(string searchString)
        {
            return RedirectToAction("PhoneVersions");
        }
    }
}