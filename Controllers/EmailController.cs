using CellableMVC.Helpers;
using CellableMVC.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CellableMVC.Controllers
{
    public class EmailController : Controller
    {
        // Get SMTP Settings from Web.config
        string FromEmailAddress = System.Configuration.ConfigurationManager.AppSettings["ConfirmationEmail"];
        string SMTPHost = System.Configuration.ConfigurationManager.AppSettings["SMTPHost"];
        int SMTPPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"]);
        string SMTPUserName = System.Configuration.ConfigurationManager.AppSettings["SMTPUserName"];
        string SMTPPassword = System.Configuration.ConfigurationManager.AppSettings["SMTPPassword"];
        string CellableAddress = System.Configuration.ConfigurationManager.AppSettings["PostalAddress"];
        private string confirmationEmail = WebConfigurationManager.AppSettings["confirmationEmail"];
        private string imageLocation = WebConfigurationManager.AppSettings["ImageLocation"];

        private CellableEntities db = new CellableEntities();
        private LoggedInUser loggedInUser = new LoggedInUser();

        public void SendEmail(int? orderId, string type, string toEmail)
        {
            string HTMLBody = "";
            string subject = "";

            try
            {
                switch (type)
                {
                    case "Confirm":
                        HTMLBody = BuildHTML(orderId);
                        subject = "Cellable Confirmation";
                        break;
                    case "Password":
                        HTMLBody = BuildPasswordHTML(toEmail);
                        subject = "Reset Cellable Password";
                        break;
                }

                // Create the Mail Message
                MailMessage mail = new MailMessage(FromEmailAddress, toEmail, subject, HTMLBody);
                // Bcc
                mail.Bcc.Add(confirmationEmail);

                //if (type == "Confirm")
                //{
                //    // Add Label as Attachment
                //    mail.Attachments.Add(new Attachment("/Pictures/SampleMailingLabel.png"));
                //}

                mail.IsBodyHtml = true;

                var client = new SmtpClient(SMTPHost, SMTPPort)
                {
                    Credentials = new NetworkCredential(SMTPUserName, SMTPPassword),
                    EnableSsl = true
                };

                // Send Confirmation Email
                client.Send(mail);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error Encountered: " + ex.Message + "<br />" + ex.InnerException;
            }
        }

        protected string BuildHTML(int? orderId)
        {
            string firstName = "";
            string lastName = "";
            string email = "";
            string oId = "";
            string amount = "";
            string paymentType = "";
            string paymentUserName = "";
            string phoneVersion = "";
            string trackingNumber = "";
            string mailLabel = "";
            string packingSheet = "https://cellableimages.blob.core.windows.net/systemimages/PackingSheet.pdf";

            var results = (from o in db.Orders.DefaultIfEmpty()
                           join up in db.UserPhones on o.UserId equals up.UserId into userPhoneGrp
                           from up in userPhoneGrp.DefaultIfEmpty()
                           join pv in db.PhoneVersions on up.VersionId equals pv.VersionId into phoneVersionsGrp
                           from pv in phoneVersionsGrp.DefaultIfEmpty()
                           join os in db.OrderStatus on o.OrderStatusId equals os.OrderStatusId into orderStatusGrp
                           from os in orderStatusGrp.DefaultIfEmpty()
                           join pt in db.PaymentTypes on o.PaymentTypeId equals pt.PaymentTypeId into paymentTypesGrp
                           from pt in paymentTypesGrp.DefaultIfEmpty()
                           join p in db.Promos on o.PromoId equals p.PromoId into promosGrp
                           from p in promosGrp.DefaultIfEmpty()
                           join ph in db.Phones on pv.PhoneId equals ph.PhoneId into phonesGrp
                           from ph in phonesGrp.DefaultIfEmpty()
                           join u in db.Users on o.UserId equals u.UserId into userGroup
                           from u in userGroup.DefaultIfEmpty()
                           where o.OrderID == orderId

                           select new {
                               firstName = u.FirstName,
                               lastName = u.LastName,
                               email = u.Email,
                               OrderId = o.OrderID,
                               amount = o.Amount,
                               phoneVersion = pv.Version,
                               paymentType = pt.PaymentType1,
                               paymentUserName = o.PaymentUserName,
                               trackingNumber = o.USPSTrackingId,
                               mailLabel = o.MailingLabel
                           });

            foreach(var item in results)
            {
                firstName = item.firstName;
                lastName = item.lastName;
                email = item.email;
                oId = item.OrderId.ToString();
                amount = decimal.Round(item.amount, 2).ToString();
                paymentType = item.paymentType;
                paymentUserName = item.paymentUserName;
                phoneVersion = item.phoneVersion;
                trackingNumber = item.trackingNumber;
                mailLabel = item.mailLabel;
            }

            string HTML = "<table style='margin-top: 50px;'>" +
                    "<tr style='border: solid; border-width: thin; background-color: black;'>" +
                        "<th style='color: white;'>First Name" +
                        "</th>" +
                        "<td style='width: 30px;'></td>" +
                        "<th style='color: white;'>Last Name " +
                        "</th>" +
                        "<td style='width: 30px;'></td>" +
                        "<th style='color: white;'>Order Number" +
                        "</th>" +
                        "<td style='width: 30px;'></td>" +
                        "<th style='color: white;'>Phone Version" +
                        "</th>" +
                        "<td style='width: 30px;'></td>" +
                        "<th style='color: white;'>Order Amount" +
                        "</th>" +
                        "<td style='width: 30px;'></td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                        firstName +
                        "</td>" +
                        "<td></td>" +
                        "<td>" +
                        lastName +
                        "</td>" +
                        "<td></td>" +
                        "<td>" +
                        oId +
                        "</td>" +
                        "<td></td>" +
                        "<td>" +
                        phoneVersion +
                        "</td>" +
                        "<td></td>" +
                        "<td>$" +
                        amount +
                        "</td>" +
                        "<td></td>" +
                    "</tr>" +
                    "<tr style='border: solid; border-width: thin; background-color: black;'>" +
                        "<th style='color: white;'>Payment Type" +
                        "</th>" +
                        "<td style='width: 30px;'></td>" +
                        "<th style='color: white'>Payment ID" +
                        "</th>" +
                        "<td style='width: 30px;'></td>" +
                        "<th style='color: white;'>Email" +
                        "</th>" +
                        "<td></td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                            paymentType +
                        "</td>" +
                        "<td style='width: 30px;'></td>" +
                        "<td>" +
                            paymentUserName +
                        "</td>" +
                        "<td style='width: 30px;'></td>" +
                        "<td>" +
                            email +
                        "</td>" +
                        "<td style='width: 30px;'></td>" +
                    "</tr>" +
                "</table>" +
                "<table style='margin-top: 30px;'>" +
                    "<tr>" +
                        "<td>" +
                            "<h3>Thank you for shopping with us!" +
                            "</h3>" +
                            "<p style='border-bottom: thin solid #000;'></p>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>" +
                            "<p>" +
                                "We're excited that you've decided to sell your gadgets to Cellable. To print your free shipping label and packing slip, print out the attachment on this email." +
                            "</p>" +
                            "<p>" +
                                "Please be sure to include your packing slip inside your box so we can easily identify your items." +
                            "</p>" +
                        "</td>" +
                    "</tr>" +
                    "<tr style='margin-top: 30px;'>" +
                        "<td>" +
                            "<h3>Shipping to Cellable</h3>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>1. Do a factory reset on the phones. This prevents missuse of data and information." +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>2. Find a strong box with plenty of packing materials to pack your items. Please do not add any items that are not on your packing slip." +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>3. Pack your items and packing slip in the box with plenty of packing materials. Tape your prepaid shipping label to the box. To print your prepaid shipping label click your attachment." +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td>4. Drop your box off for shipping at the nearest USPS location." +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td><b>Click <a href='" + mailLabel + "'>HERE</a> to download your shipping label</b>" +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td><b>Click <a href='" + packingSheet + "'>HERE</a> to download your packing slip</b>" +
                        "</td>" +
                    "</tr>" +
                "</table>" +
                "<table>" +
                    "<tr style='margin-top: 30px;'>" +
                        "<td>We'll let you know as soon as we receive your items. If you have any questions along the way, you can check the status of your items on our website or visit our help center." +
                        "</td>" +
                    "</tr>" +
                    "<tr>" +
                        "<td style='height: 36px'>" +
                            "<p style='margin-top: 32px;'>" +
                                "Thanks,</p>" +
                                    "<p style='margin-left: 35px;'>  Cellable team" +
                            "</p>" +
                        "</td>" +
                    "</tr>" +
                    "<tr style='margin-top: 50px;'>" +
                        "<td style='margin-top: 30px;'>" +
                            "<p style='text-align: center; margin-top: 30px; font-size: xx-small;'>" +
                                "© Copyright 2020 - Cellable LLC, All Rights Reserved, Patents Pending." +
                            "</p>" +
                            "<p style='text-align: center;  font-size: xx-small;'>" +
                                "Designated trademarks and brands are the property of their respective owners." +
                            "</p>" +
                            "<p style='text-align: center; font-size: xx-small;'>" +
                                "Cellable is not affiliated with the manufacturers of the items available for trade-in." +
                            "</p>" +
                            "<p style='text-align: center; font-size: xx-small;'>" +
                                CellableAddress +
                            "</p>" +
                        "</td>" +
                    "</tr>" +
                "</table>";

            return HTML;
        }

        protected string BuildPasswordHTML(string userEmail)
        {
            string HTML;

            // Get Web Site's Base URL
            string baseUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string url = baseUrl.Replace("/Users/Login", "/Users/ForgotPassword");

            HTML = "<html>" +
                        "<head>" +
                        "</head>" +
                        "<body>" +
                        "<table>" +
                            "<tr>" +
                                "<td>" +
                                    "We received a request to reset the password associated with this email address. " +
                                    "If you made this request, please follow these instructions." +
                                    "<p>" +
                                    "Click this link to reset your password using our secure server." +
                                    "<p>" +
                                    "<a href='" + url + "?email=" + userEmail + "'> Reset Password</a>" +
                                    "<p>" +
                                    "If clicking on the link doesn't work, copy and paste it into the address window of your browser or retype it there." +
                                    "<p>" +
                                    "If you did not make this request, please ignore this email." +
                                    "<p>" +
                                    "Thank you for using Cellable!" +
                                "</td>" +
                            "</tr>" +
                        "</table>" +
                        "</body>" +
                    "</html>";

            return HTML;
        }
    }
}