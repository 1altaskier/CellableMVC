using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CellableMVC.Helpers;

namespace CellableMVC.Controllers
{
    public class PhonesController : Controller
    {
        private CellableEntities db = new CellableEntities();
        private IncomingPhone phone = new IncomingPhone();

        // GET: Phones
        public ActionResult Phones()
        {
            // Clear Previously Set Phone Related Session Variables
            Session["BrandId"] = null;
            Session["BrandName"] = null;
            Session["VersionId"] = null;
            Session["VersionName"] = null;
            Session["BaseCost"] = null;
            Session["Phone Value"] = null;
            Session["ImageLocation"] = null;
            Session["PhoneBrandName"] = null;
            Session["PromoCode"] = null;
            Session["PromoValue"] = null;

            // Same as Above, Except Using IncomingPhone Class
            phone.BrandId = null;
            phone.BrandName = null;
            phone.VersionId = null;
            phone.VersionName = null;
            phone.BaseCost = null;
            phone.PhoneValue = null;
            phone.ImageLocation = null;
            phone.PhoneBrandName = null;
            phone.PromoCode = null;
            phone.PromoValue = null;

            var phones = db.Phones.ToList();

            return View(phones);
        }

        public ActionResult Carriers(int? id, int? versionId, string versionName = null)
        {
            Phone phoneBrand = db.Phones.Find(id);
            Session["BrandId"] = id;
            Session["BrandName"] = phoneBrand.Brand;

            if (versionId != null)
            {
                Session["VersionId"] = versionId;
                Session["VersionName"] = versionName;
            }

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
            possibleDefects = db.PossibleDefects.ToList().OrderBy(x => x.DefectGroupId).Where(x => x.VersionId == id).ToList();

            // Get the Version Info for this Particular Phone
            PhoneVersion phoneVersion = db.PhoneVersions.Find(id);

            // Update Phone Version View Count to DB
            if (phoneVersion.Views == null)
            {
                phoneVersion.Views = 1;
            }
            else
            {
                phoneVersion.Views = phoneVersion.Views + 1;
            }
            db.SaveChanges();

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

            ViewBag.Capacities = db.StorageCapacities.ToList();
            ViewBag.PhoneCapacities = db.VersionCapacities.Where(x => x.VersionId == phoneVersion.VersionId).ToList();


            //var context = new CellableEntities();
            //using (context)
            //{
            //    var storage = (from vc in db.VersionCapacities.DefaultIfEmpty()
            //                   join sc in db.StorageCapacities on vc.StorageCapacityId equals sc.StorageCapacityId into capacitiesGrp
            //                   from sc in capacitiesGrp.DefaultIfEmpty()
            //                   where vc.VersionId == phoneVersion.VersionId
            //                   select new { vc.StorageCapacityId, sc.Description }).ToList();

            //    ViewBag.Storage = storage;

            //    LoggedInUser u = new LoggedInUser();
            //    u.StorageCapacities = storage;
            //}

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
                    Session["PromoCodeId"] = promo.PromoId;
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

        public ActionResult SearchResults(string searchString)
        {
            IList<PhoneVersion> versions = db.PhoneVersions
                                            .ToList()
                                            .Where(x => x.Version.ToLower().Contains(searchString.ToLower())
                                                || x.Phone.Brand.ToLower().Contains(searchString.ToLower()))
                                            .ToList();

            return View(versions);
        }
    }
}