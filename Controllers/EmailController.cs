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
        // Get SMTP Settings from Web.config
        string FromEmailAddress = System.Configuration.ConfigurationManager.AppSettings["ConfirmationEmail"];
        string SMTPHost = System.Configuration.ConfigurationManager.AppSettings["SMTPHost"];
        int SMTPPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"]);
        string SMTPUserName = System.Configuration.ConfigurationManager.AppSettings["SMTPUserName"];
        string SMTPPassword = System.Configuration.ConfigurationManager.AppSettings["SMTPPassword"];

        private string confirmationEmail = WebConfigurationManager.AppSettings["confirmationEmail"];

        public void SendEmail(string type, string toEmail)
        {
            string HTMLBody = "";
            string subject = "";

            try
            {
                switch (type)
                {
                    case "Confirm":
                        HTMLBody = BuildHTML();
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

                if (type == "Confirm")
                {
                    // Add Label as Attachment
                    mail.Attachments.Add(new Attachment(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures/SampleMailingLabel.png")));
                }

                mail.IsBodyHtml = true;

                var client = new SmtpClient(SMTPHost, SMTPPort)
                {
                    Credentials = new NetworkCredential(SMTPUserName, SMTPPassword),
                    EnableSsl = true
                };

                // Send Confirmation Email
                client.Send(mail);
            }
            catch(Exception ex)
            {
                ViewBag.Message = "Error Encountered: " + ex.Message + "<br />" + ex.InnerException;
            }
        }

        public void ConfirmationEmail(string EmailAddress)
        {
            try
            {
                EmailAddress = "cellablewebdev@gmail.com";

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

        public void ResetPassword(string Email)
        {
            try
            {
                string HTMLBody = BuildPasswordHTML(Email);

                // Create the Mail Message
                MailMessage mail = new MailMessage(FromEmailAddress, Email, "Cellable Confirmation ", HTMLBody);
                // Bcc
                mail.Bcc.Add(confirmationEmail);
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
                                    "<td align=\"left\" valign=\"top\"><div class=\"smaller\">&copy;" + @DateTime.Now.Year + " - Cellable</div></td>" +
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

        protected string BuildPasswordHTML(string userEmail)
        {
            string HTML;

            // Get Web Site's Base URL
            string baseUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            baseUrl.Replace("/Email/SendEmail", "/User/ForgotPassword");

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
                                    "<a href='" + baseUrl + "?email=" + userEmail + "'> Reset Password</a>" +
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