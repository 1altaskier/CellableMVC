using CellableMVC.Mail;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CellableMVC.Controllers
{
    public class MailController : Controller
    {
        private string USPSAPIUserName = WebConfigurationManager.AppSettings["USPSAPIUserName"];
        private string USPSAPIPassword = WebConfigurationManager.AppSettings["USPSAPIPassword"];

        // GET: Mail
        public void ValidateAddress(string address, string city, string state)
        {
            ///Create a new instance of the USPS Manager class
            ///The constructor takes 2 arguments, the first is
            ///your USPS Web Tools User ID and the second is 
            ///true if you want to use the USPS Test Servers.
            USPSManager m = new USPSManager(USPSAPIUserName, true);
            Address a = new Address();
            a.Address2 = "6406 Ivy Lane";
            a.City = "Greenbelt";
            a.State = "MD";

            ///By calling ValidateAddress on the USPSManager object,
            ///you get an Address object that has been validated by the
            ///USPS servers
            Address validatedAddress = m.ValidateAddress(a);
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

        public void TrackPackage(string trackingNumber)
        {
            USPSManager m = new USPSManager(USPSAPIUserName, true);
            TrackingInfo t = m.GetTrackingInfo(USPSAPIUserName, USPSAPIPassword, "EJ958083578US");
        }

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