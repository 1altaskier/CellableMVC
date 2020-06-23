using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CellableMVC.Helpers;
using System.Web.Configuration;

namespace CellableMVC.Controllers
{
    public class PhonesController : Controller
    {
        private CellableEntities db = new CellableEntities();
        private IncomingPhone phone = new IncomingPhone();

        private string imageLocation = WebConfigurationManager.AppSettings["ImageLocation"];
        private string phoneImageLocation = WebConfigurationManager.AppSettings["PhoneImageLocation"];

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

            var title = db.SystemSettings.Find(15);
            var text = db.SystemSettings.Find(21);
            var footer = db.SystemSettings.Find(22);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;

            ViewBag.imageLocation = imageLocation;

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

            var title = db.SystemSettings.Find(43);
            var text = db.SystemSettings.Find(16);
            var footer = db.SystemSettings.Find(20);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;

            ViewBag.imageLocation = imageLocation;

            IList<Carrier> carriers = db.Carriers.Where(x => x.Active == true).OrderBy(x => x.Order).ToList();

            return View(carriers);
        }

        public ActionResult PhoneVersions(int? brandId, int? carrierId, string searchString = null)
        {
            Session["CarrierValue"] = Request["value"]; ;

            // Initialize PhoneVersions Variable
            IList<PhoneVersion> phoneVersions = null;

            bool? active = true;

            if (searchString == null)
            {
                // Get entire list of Phone Versions to pass to the view
                phoneVersions = db.PhoneVersions.ToList().Where(x => x.Phone.PhoneId == brandId && x.Active == active).ToList();
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

            var title = db.SystemSettings.Find(25);
            var text = db.SystemSettings.Find(23);
            var footer = db.SystemSettings.Find(24);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;

            ViewBag.imageLocation = phoneImageLocation;

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

            // Update Phone Version View Count to DB
            if (phoneVersion.Views == null)
            {
                phoneVersion.Views = 1;
            }
            else
            {
                phoneVersion.Views += 1;
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

            var title = db.SystemSettings.Find(26);
            var text = db.SystemSettings.Find(44);
            var footer = db.SystemSettings.Find(45);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;

            ViewBag.imageLocation = imageLocation;
            ViewBag.phoneImageLocation = phoneImageLocation;

            return View(possibleDefects);
        }

        public ActionResult PricePhone(FormCollection form)
        {
            IList<PossibleDefect> defects = db.PossibleDefects.ToList();

            // Get Phone's Base Value & Capacity Description
            decimal? baseCost = null;
            if (Request["hdnCapacity"] != null)
            {
                int carrierId = int.Parse(Session["Carrier"].ToString());
                int versionId = int.Parse(Session["VersionId"].ToString());

                // Get Carrier Cost
                var versionCarrier = db.VersionCarriers.First(x => x.CarrierId == carrierId && x.VersionId == versionId);

                baseCost = int.Parse(Request["hdnCapacity"]);
                baseCost -= decimal.Parse(versionCarrier.Value.ToString());
                Session["CapacityDescription"] = Request["hdnCapacityDesc"];
                Session["BaseValue"] = baseCost;
            }
            else
            {
                baseCost = decimal.Parse(Session["BaseValue"].ToString());
            }

            var i = 0;
            foreach (var item in form)
            {
                if (item.ToString() != "__RequestVerificationToken" &&
                    item.ToString() != "id" &&
                    item.ToString() != "capacity" &&
                    item.ToString() != "carriers" &&
                    item.ToString() != "hdCarrier" &&
                    item.ToString() != "hdnCapacity" &&
                    item.ToString() != "hdnCapacityDesc" &&
                    !item.ToString().Contains("val_") &&
                    !item.ToString().Contains("hdn_"))
                {
                    string defectField = Request.Form[item.ToString()];
                    string[] delimiter = new string[] { "_" };
                    string[] defect;
                    defect = defectField.Split(delimiter, StringSplitOptions.None);

                    if (defect[2] != "0" && defect[2] != "0.00")
                    {
                        baseCost -= decimal.Parse(defect[2]);
                        Session[defect[1]] = defect[2];

                        Session["QuestionAnswer" + i] = defect[0] + "_" + defect[1] + "_" + defect[2];

                        i++;
                    }
                }
            }

            if (!String.IsNullOrEmpty(Request.Form["capacity"]))
            {
                Session["Storage Capacity"] = Request.Form["capacity"];
            }

            // For Description on Pricing Page
            ViewBag.CapacityDesc = Session["CapacityDescription"];

            if (Session["Phone Value"] == null)
            {
                Session["Phone Value"] = baseCost < 0 ? 0 : baseCost;

                if (baseCost < 0)
                {
                    Session["Phone Value"] = 0;
                }
                else
                {
                    Session["Phone Value"] = baseCost;
                }
            }

            var title = db.SystemSettings.Find(27);
            var text = db.SystemSettings.Find(28);
            var footer = db.SystemSettings.Find(29);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;

            ViewBag.imageLocation = phoneImageLocation;

            return View(defects);
        }

        public ActionResult CalcPromo(string PromoCode)
        {
            if (!String.IsNullOrEmpty(PromoCode))
            {
                try
                {
                    Promo promo = db.Promos.FirstOrDefault(x => x.PromoCode == PromoCode && (x.StartDate < DateTime.Today && x.EndDate > DateTime.Today));

                    decimal? promoDiscount = 0;
                    if (promo.Discount != 0)
                    {
                        promoDiscount = decimal.Parse(Session["Phone Value"].ToString()) * promo.Discount;
                        Session["PromoValue"] = promo.Discount;
                        Session["Phone Value"] = decimal.Parse(Session["Phone Value"].ToString()) + promoDiscount;
                        Session["PromoType"] = "%";
                    }
                    else
                    {
                        promoDiscount = decimal.Parse(Session["Phone Value"].ToString()) + promo.DollarValue;
                        Session["PromoValue"] = promo.DollarValue;
                        Session["Phone Value"] = promoDiscount;
                        Session["PromoType"] = "$";
                    }

                    Session["PromoCodeId"] = promo.PromoId;
                    Session["PromoCode"] = PromoCode;
                }
                catch (Exception ex)
                {

                }
            }

            return RedirectToAction("PricePhone", "Phones");
        }

        public ActionResult SearchResults(string searchString)
        {
            var title = db.SystemSettings.Find(46);
            var text = db.SystemSettings.Find(47);
            var footer = db.SystemSettings.Find(48);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;
            ViewBag.phoneImageLocation = phoneImageLocation;

            // Initialize PhoneVersions Variable
            IList<PhoneVersion> phoneVersions = null;

            phoneVersions = db.PhoneVersions.ToList().Where(x => x.Version.ToLower().Contains(searchString.ToLower())
                                                                && x.Active == true).ToList();

            return View(phoneVersions);
        }
    }
}