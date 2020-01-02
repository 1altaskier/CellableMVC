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
            //IList<DefectGroup> defectGroup = db.DefectGroups.ToList();

            // Get a list of Defects to pass to the view
            possibleDefects = db.PossibleDefects.ToList().OrderBy(x => x.DefectGroup.DisplayOrder).Where(x => x.VersionId == id).ToList();

            PhoneVersion phoneVersion = db.PhoneVersions.Find(id);

            IList<Carrier> list = db.Carriers.ToList();
            ViewBag.Carriers = list;

            // Get Phone Image to display
            ViewBag.ImageLocation = phoneVersion.ImageName;
            ViewBag.VersionName = phoneVersion.Version;

            Session["VersionName"] = phoneVersion.Version;
            Session["ImageLocation"] = phoneVersion.ImageName;
            Session["VersionId"] = phoneVersion.VersionId;
            Session["BaseCost"] = phoneVersion.BaseCost;

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
                if(item.ToString() == "capacity")
                {
                    Session["Storage Capacity"] = Request.Form[item.ToString()];
                }
                if(item.ToString() == "carriers")
                {
                    Session["Carrier"] = Request.Form[item.ToString()];
                }
            }

            if (Session["Phone Value"] == null)
            {
                Session["Phone Value"] = baseCost;
            }

            ViewBag.Carriers = new SelectList(db.Carriers, "CarrierId", "CarrierName", "-- Select Carrier --");

            return View(defects);
        }

        public ActionResult CalcPromo(string PromoCode)
        {
            try
            {
                Promo promo = db.Promos.FirstOrDefault(x => x.PromoCode == PromoCode && (x.StartDate < DateTime.Today && x.EndDate > DateTime.Today));

                var promoDiscount = decimal.Parse(Session["Phone Value"].ToString()) * promo.Discount;
                Session["PromoCode"] = PromoCode;
                Session["PromoValue"] = promo.Discount;
                Session["Phone Value"] = decimal.Parse(Session["Phone Value"].ToString()) + promoDiscount;
            }
            catch(Exception ex)
            {

            }

            return RedirectToAction("PricePhone", "Phones");
        }
    }
}