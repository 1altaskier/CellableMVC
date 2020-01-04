using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CellableMVC.Controllers
{
    public class EmailController : Controller
    {
        private string confirmationEmail = WebConfigurationManager.AppSettings["confirmationEmail"];
        public void ConfirmationEmail(string EmailAddress)
        {
            try
            {
                EmailAddress = "cellablewebdev@gmail.com";

                // Get SMTP Settings from Web.config
                string FromEmailAddress = System.Configuration.ConfigurationManager.AppSettings["ConfirmationEmail"];
                string SMTPHost = System.Configuration.ConfigurationManager.AppSettings["SMTPHost"];
                int SMTPPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"]);
                string SMTPUserName = System.Configuration.ConfigurationManager.AppSettings["SMTPUserName"];
                string SMTPPassword = System.Configuration.ConfigurationManager.AppSettings["SMTPPassword"];
                string HTMLBody = BuildHTML();

                // Create the Mail Message
                MailMessage mail = new MailMessage(FromEmailAddress, EmailAddress, "Cellable Confirmation ", HTMLBody);
                // Bcc
                mail.Bcc.Add(confirmationEmail);
                // Add Label as Attachment
                mail.Attachments.Add(new Attachment(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures/SampleMailingLabel.png")));
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

        protected string BuildHTML()
        {
            string HTML;

            HTML = "<html>" +
                    "<head>" +
                        "<title></title>" +
                        "<title></title>" +
                          "<style>" +
                            "img.resize{" +
                                  "max-width: 75%;" +
                                  "max-height: 75%;" +
                             "}" +
                             "body{" +
                                "font-family: arial;" +
                                "font-size:medium;" +
                            "}" +
                            "div.small{" +
                                "font-size:small;" +
                            "}" +
                            "div.smaller{" +
                                "font-size:smaller;" +
                            "}" +
                            "div.grayColor{" +
                                "color:dimgray;" +
                            "}" +
                    "</style>" +
                    "</head> " +
                    "<body>" +
                       "<table style=\"width:90%; background-color:darkgreen; font-family:Arial; color:antiquewhite\" align=\"center\"> " +
                              "<tr> " +
                                  "<td width=\"50%\" align=\"center\" valign=\"top\">" +
                                        "<p>" +
                                            "<div class=\"grayColor\"><b>USPS Tracking Number</b></div>EJ958083578US" +
                                        "</p>" +
                                        "<hr />" +
                                        "<table style=\"width:100%;\">" +
                                             "<tr >" +
                                                "<td >" +
                                                    "<b > Phone VAlue: $150</b>" +
                                                    "<p></p>" +
                                                "</td>" +
                                            "</tr>" +
                                            "<tr>" +
                                                "<td>" +
                                                    "Your phone information has been added to our system..." +
                                                "</td>" +
                                            "</tr>" +
                                        "</table>" +
                                    "</td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td colspan=\"2\"><hr/></td>" +
                                "</tr>" +
                                "<tr>" +
                                    "<td align=\"left\" valign=\"top\"><div class=\"smaller\">&copy;" +  @DateTime.Now.Year + " - Cellable</div></td>" +
                                    "<td align=\"right\" valign=\"top\" class=\"smaller\">" +
                                         "<div class=\"smaller\"><a href=\"mailto:contactus@cellable.com\">contactus@cellable.com</a>" +
                                         "<br />" +
                                         "(123)&nbsp;456-7890</div>" +
                                    "</td>" +
                                "</tr>" +
                                "</table>" +
                        "</body>" +
                        "</html>";

            return HTML;
        }

    }
}