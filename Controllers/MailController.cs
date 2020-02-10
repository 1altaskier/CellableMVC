using CellableMVC.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using CellableMVC.Helpers;
using Shippo;
using System.Collections;
using System;
using System.Collections.Generic;
using CellableMVC.Models;
using System.Data.Entity;

namespace CellableMVC.Controllers
{
    public class MailController : Controller
    {
        private CellableEntities db = new CellableEntities();

        private string USPSAPIUserName = WebConfigurationManager.AppSettings["USPSAPIUserName"];
        private string USPSAPIPassword = WebConfigurationManager.AppSettings["USPSAPIPassword"];
        private string ShippoTestAPIToken = WebConfigurationManager.AppSettings["ShippoTestAPIToken"];
        private string ContactUsPhone = WebConfigurationManager.AppSettings["ContactUsPhone"];
        private string ContactEmail = WebConfigurationManager.AppSettings["ContactEmail"];

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

        public void GetShippingLabel(int userId, int orderId)
        {
            // Generate Mailing Label
            APIResource resource = new APIResource(ShippoTestAPIToken);

            // To Address
            //Get Cellable Mail Info
            SystemSetting address = db.SystemSettings.Find(9);
            SystemSetting city = db.SystemSettings.Find(11);
            SystemSetting state = db.SystemSettings.Find(12);
            SystemSetting zip = db.SystemSettings.Find(13);
            SystemSetting phone = db.SystemSettings.Find(14);

            Hashtable toAddressTable = new Hashtable();
            toAddressTable.Add("name", "Cellable Receiving");
            toAddressTable.Add("company", "Cellable");
            toAddressTable.Add("street1", address.Value);
            toAddressTable.Add("city", city.Value);
            toAddressTable.Add("state", state.Value);
            toAddressTable.Add("zip", zip.Value);
            toAddressTable.Add("country", "US");
            toAddressTable.Add("phone", "+1 " + ContactUsPhone);
            toAddressTable.Add("email", ContactEmail);

            // from address
            // Get User Mail Info
            User user = db.Users.Find(userId);
            Hashtable fromAddressTable = new Hashtable();
            fromAddressTable.Add("name", user.FirstName + " " + user.LastName);
            fromAddressTable.Add("street1", user.Address);
            fromAddressTable.Add("city", user.City);
            fromAddressTable.Add("state", user.State);
            fromAddressTable.Add("zip", user.Zip);
            fromAddressTable.Add("country", "US");
            fromAddressTable.Add("email", user.Email);
            fromAddressTable.Add("phone", "+1 " + user.PhoneNumber);
            fromAddressTable.Add("metadata", "Order ID " + orderId);

            // parcel
            Hashtable parcelTable = new Hashtable();
            parcelTable.Add("length", "6");
            parcelTable.Add("width", "4");
            parcelTable.Add("height", "2");
            parcelTable.Add("distance_unit", "in");
            parcelTable.Add("weight", "7");
            parcelTable.Add("mass_unit", "oz");
            List<Hashtable> parcels = new List<Hashtable>();
            parcels.Add(parcelTable);


            // shipment
            Hashtable shipmentTable = new Hashtable();
            shipmentTable.Add("address_to", toAddressTable);
            shipmentTable.Add("address_from", fromAddressTable);
            shipmentTable.Add("parcels", parcels);
            shipmentTable.Add("object_purpose", "PURCHASE");
            shipmentTable.Add("async", false);

            // create Shipment object
            Shipment shipment = resource.CreateShipment(shipmentTable);

            // select desired shipping rate according to your business logic
            // we simply select the first rate in this example
            Rate rate = shipment.Rates[0];

            Hashtable transactionParameters = new Hashtable();
            transactionParameters.Add("rate", rate.ObjectId);
            transactionParameters.Add("async", false);
            Transaction transaction = resource.CreateTransaction(transactionParameters);

            if (((String)transaction.Status).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
            {
                var order = new Order() 
                { 
                    OrderID = orderId,
                    MailingLabel = transaction.LabelURL.ToString(),
                    USPSTrackingId = transaction.TrackingNumber.ToString()
                };
                using (var db = new CellableEntities())
                {
                    db.Orders.Attach(order);
                    db.Entry(order).Property(x => x.MailingLabel).IsModified = true;
                    db.Entry(order).Property(x => x.USPSTrackingId).IsModified = true;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                }

                // Save Label URL & Tracking # to the Database
                //Order order = new Order();
                //order.OrderID = orderId;
                //order.MailingLabel = transaction.LabelURL.ToString();
                //order.USPSTrackingId = transaction.TrackingNumber.ToString();
                //order.CreateDate = DateTime.Now;

                //db.Orders.Attach(order);

                //db.Entry(order).Property(x => x.Amount).IsModified = false;
                //db.Entry(order).Property(x => x.UserId).IsModified = false;
                //db.Entry(order).Property(x => x.OrderStatusId).IsModified = false;
                //db.Entry(order).Property(x => x.CreateDate).IsModified = false;
                //db.Entry(order).Property(x => x.CreateBy).IsModified = false;
                //db.Entry(order).Property(x => x.PaymentTypeId).IsModified = false;
                //db.Entry(order).Property(x => x.PromoId).IsModified = false;
                //db.Entry(order).Property(x => x.UserPhoneId).IsModified = false;
                //db.Entry(order).Property(x => x.PaymentUserName).IsModified = false;

                //db.Entry(order).Property(x => x.MailingLabel).IsModified = true;
                //db.Entry(order).Property(x => x.USPSTrackingId).IsModified = true;

                //db.Entry(order).State = EntityState.Modified;
                //db.Configuration.ValidateOnSaveEnabled = false;
                //db.SaveChanges();
            }
            else
            {
                Console.WriteLine("An Error has occured while generating your label. Messages : " + transaction.Messages);
            }
        }
    }
}