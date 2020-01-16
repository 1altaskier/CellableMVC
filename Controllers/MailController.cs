using CellableMVC.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using CellableMVC.Helpers;

namespace CellableMVC.Controllers
{
    public class MailController : Controller
    {
        private string USPSAPIUserName = WebConfigurationManager.AppSettings["USPSAPIUserName"];
        private string USPSAPIPassword = WebConfigurationManager.AppSettings["USPSAPIPassword"];

        public ActionResult _USPSTrackingMessage(string trackingNumber)
        {
            USPSManager mgr = new USPSManager(USPSAPIUserName, true);
            string msg = mgr.GetTrackingInfo(USPSAPIUserName, USPSAPIPassword, trackingNumber);

            MailHelper.TrackingMessage = msg;

            return View();
        }

        // GET: Mail
        public bool ValidateAddress(string address, string city, string state)
        {
            bool valid = true;

            ///Create a new instance of the USPS Manager class
            ///The constructor takes 2 arguments, the first is
            ///your USPS Web Tools User ID and the second is 
            ///true if you want to use the USPS Test Servers.
            USPSManager mgr = new USPSManager(USPSAPIUserName, true);
            Address a = new Address();
            a.Address2 = address;
            a.City = city;
            a.State = state;

            valid = mgr.ValidateAddress(USPSAPIUserName, a);

            return valid;
        }

        public void GetZipCode(string address, string city, string state)
        {
            USPSManager m = new USPSManager(USPSAPIUserName, true);
            Address a = new Address();
            a.Address2 = "6406 Ivy Lane";
            a.City = "Greenbelt";
            a.State = "MD";
            Address addressWithZip = m.GetZipcode(a);
            string zip = addressWithZip.Zip;
        }

        public void GetCityStateFromZip(int zip)
        {
            USPSManager m = new USPSManager(USPSAPIUserName, true);
            Address a = m.GetCityState(zip.ToString());
            string outCity = a.City;
            string outState = a.State;
        }

        //public ActionResult TrackPackage(string trackingNumber)
        //{
        //    USPSManager mgr = new USPSManager(USPSAPIUserName, true);
        //    string msg = mgr.GetTrackingInfo(USPSAPIUserName, USPSAPIPassword, trackingNumber);

        //    Helpers.MailHelper mail = new Helpers.MailHelper();
        //    mail.TrackingMessage = msg;

        //    return RedirectToAction("TrackOrders", "Users", new { } );
        //}

        public void GetShippingLabel()
        {
            USPSManager m = new USPSManager(USPSAPIUserName, true);
            Package p = new Package();
            p.FromAddress.Contact = "John Smith";
            p.FromAddress.Address2 = "475 L'Enfant Plaza, SW";
            p.FromAddress.City = "Washington";
            p.FromAddress.State = "DC";
            p.FromAddress.Zip = "20260";
            p.ToAddress.Contact = "Tom Customer";
            p.ToAddress.Address1 = "STE 201";
            p.ToAddress.Address2 = "6060 PRIMACY PKWY";
            p.ToAddress.City = "Memphis";
            p.ToAddress.State = "TN";
            p.WeightInOunces = 2;
            p.ServiceType = ServiceType.Priority;
            p.SeparateReceiptPage = false;
            p.LabelImageType = LabelImageType.TIF;
            p.PackageSize = PackageSize.Regular;
            p.PackageType = PackageType.Flat_Rate_Box;
            p = m.GetDeliveryConfirmationLabel(p);
        }
    }
}