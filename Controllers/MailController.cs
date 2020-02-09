using CellableMVC.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using CellableMVC.Helpers;
using Shippo;
using System.Collections;
using System;

namespace CellableMVC.Controllers
{
    public class MailController : Controller
    {
        private string USPSAPIUserName = WebConfigurationManager.AppSettings["USPSAPIUserName"];
        private string USPSAPIPassword = WebConfigurationManager.AppSettings["USPSAPIPassword"];
        private string ShippoTestAPIToken = WebConfigurationManager.AppSettings["ShippoTestAPIToken"];

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
            Mail.Address a = new Mail.Address();
            a.Address2 = address;
            a.City = city;
            a.State = state;

            valid = mgr.ValidateAddress(USPSAPIUserName, a);

            return valid;
        }

        public void GetZipCode(string address, string city, string state)
        {
            USPSManager m = new USPSManager(USPSAPIUserName, true);
            Mail.Address a = new Mail.Address();
            a.Address2 = "6406 Ivy Lane";
            a.City = "Greenbelt";
            a.State = "MD";
            Mail.Address addressWithZip = m.GetZipcode(a);
            string zip = addressWithZip.Zip;
        }

        public void GetCityStateFromZip(int zip)
        {
            USPSManager m = new USPSManager(USPSAPIUserName, true);
            Mail.Address a = m.GetCityState(zip.ToString());
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
            // Generate Mailing Label
            https://api.goshippo.com/
            APIResource resource = new APIResource(ShippoTestAPIToken);

            // to address
            Hashtable toAddressTable = new Hashtable();
            toAddressTable.Add("name", "Mr Hippo");
            toAddressTable.Add("company", "Shippo");
            toAddressTable.Add("street1", "215 Clayton St.");
            toAddressTable.Add("city", "San Francisco");
            toAddressTable.Add("state", "CA");
            toAddressTable.Add("zip", "94117");
            toAddressTable.Add("country", "US");
            toAddressTable.Add("phone", "+1 555 341 9393");
            toAddressTable.Add("email", "support@goshipppo.com");

            // from address
            Hashtable fromAddressTable = new Hashtable();
            fromAddressTable.Add("name", "Ms Hippo");
            fromAddressTable.Add("company", "San Diego Zoo");
            fromAddressTable.Add("street1", "2920 Zoo Drive");
            fromAddressTable.Add("city", "San Diego");
            fromAddressTable.Add("state", "CA");
            fromAddressTable.Add("zip", "92101");
            fromAddressTable.Add("country", "US");
            fromAddressTable.Add("email", "hippo@goshipppo.com");
            fromAddressTable.Add("phone", "+1 619 231 1515");
            fromAddressTable.Add("metadata", "Customer ID 123456");

            // parcel
            Hashtable parcelTable = new Hashtable();
            parcelTable.Add("length", "5");
            parcelTable.Add("width", "5");
            parcelTable.Add("height", "5");
            parcelTable.Add("distance_unit", "in");
            parcelTable.Add("weight", "2");
            parcelTable.Add("mass_unit", "lb");

            // shipment
            Hashtable shipmentTable = new Hashtable();
            shipmentTable.Add("address_to", toAddressTable);
            shipmentTable.Add("address_from", fromAddressTable);
            shipmentTable.Add("parcels", parcelTable);

            Console.WriteLine("Getting shipping label..");
            Hashtable transactionParameters = new Hashtable();
            transactionParameters.Add("shipment", shipmentTable);
            transactionParameters.Add("servicelevel_token", "usps_priority");
            transactionParameters.Add("carrier_account", "b741b99f95e841639b54272834bc478c");

            try
            {
                Transaction transaction = resource.CreateTransaction(transactionParameters);

                if (((String)transaction.Status).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Label url : " + transaction.LabelURL);
                    Console.WriteLine("Tracking number : " + transaction.TrackingNumber);
                }
                else
                {
                    Console.WriteLine("An Error has occured while generating your label. Messages : " + transaction.Messages);
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}