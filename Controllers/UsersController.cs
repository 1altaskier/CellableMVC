using CellableMVC.Mail;
using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CellableMVC.Helpers;
using System.Data;
using System.Data.Entity;
using Shippo;
using System.Collections;

namespace CellableMVC.Controllers
{
    public class UsersController : Controller
    {
        private string USPSAPIUserName = WebConfigurationManager.AppSettings["USPSAPIUserName"];
        private string USPSAPIPassword = WebConfigurationManager.AppSettings["USPSAPIPassword"];
        private string AdminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
        private string phoneImageLocation = WebConfigurationManager.AppSettings["PhoneImageLocation"];
        private string ShippoLiveAPIToken = WebConfigurationManager.AppSettings["ShippoLiveAPIToken"];

        private CellableEntities db = new CellableEntities();

        private LoggedInUser loggedInUser = new LoggedInUser();

        [HttpPost]
        public JsonResult UserExists(string UserName)
        {
            if (UserName != null)
            {
                if (db.Users.Any(x => x.UserName == UserName))
                {
                    User existingUser = db.Users.Single(x => x.UserName == UserName);
                    if (existingUser.UserName == UserName)
                    {
                        return Json(false);
                    }
                    else
                    {
                        return Json(true);
                    }
                }
                else
                {
                    return Json(true);
                }
            }
            else
            {
                return Json(!db.Users.Any(x => x.UserName == UserName));
            }
        }

        [HttpPost]
        public JsonResult EmailExists(string Email)
        {
            if (Email != null)
            {
                if (db.Users.Any(x => x.Email == Email))
                {
                    User existingEmail = db.Users.Single(x => x.Email == Email);
                    if (existingEmail.Email == Email)
                    {
                        return Json(false);
                    }
                    else
                    {
                        return Json(true);
                    }
                }
                else
                {
                    return Json(true);
                }
            }
            else
            {
                return Json(!db.Users.Any(x => x.Email == Email));
            }
        }

        // GET: Users
        public ActionResult Index()
        {
            var model = new User();

            return View(model);
        }

        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult CompleteUserPhoneRegistration()
        {
            return RedirectToAction("Register", "Users");
        }

        // GET: Users/Create
        public ActionResult Register(string msg = null)
        {
            if (Session["LoggedInUser"] != null)
            {
                return RedirectToAction("ReturningUser");
            }

            ViewBag.Message = msg;
            ViewBag.PaymentTypes = new SelectList(db.PaymentTypes, "PaymentTypeId", "PaymentType1", "-- How You Get Paid --");
            ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", "-- Select State --");

            var title = db.SystemSettings.Find(30);
            var text = db.SystemSettings.Find(31);
            var footer = db.SystemSettings.Find(32);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;

            return View();
        }

        public ActionResult ReturningUser()
        {
            IList<PossibleDefect> defects = db.PossibleDefects.ToList();
            ViewBag.PossibleDefects = defects;

            ViewBag.PaymentTypes = new SelectList(db.PaymentTypes, "PaymentTypeId", "PaymentType1", "-- How You Get Paid --");

            //StorageCapacity capacity = db.StorageCapacities.Find(int.Parse(Session["Storage Capacity"].ToString()));
            // For Description on Pricing Page
            ViewBag.CapacityDesc = Session["CapacityDescription"];

            User user = db.Users.Find(int.Parse(Session["LoggedInUserId"].ToString()));

            var title = db.SystemSettings.Find(36);
            var text = db.SystemSettings.Find(37);
            var footer = db.SystemSettings.Find(38);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;
            ViewBag.phoneImageLocation = phoneImageLocation;

            return View(user);
        }

        public ActionResult UpdateReturningUser(string errMsg = null)
        {
            LoggedInUser loggedInUser = new LoggedInUser();

            // Get User Info From DB
            User user = db.Users.Find(int.Parse(Session["LoggedInUserId"].ToString()));

            // Create Drop Down List(s)
            var state = user.State;
            ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", state);

            ViewBag.Message = errMsg;

            return View(user);
        }

        public ActionResult DeleteUser()
        {
            int userId = int.Parse(Session["LoggedInUserId"].ToString());

            // See if User has any previously sold phones
            IList<UserPhone> userPhones = db.UserPhones.Where(x => x.UserId == userId).ToList();

            if (userPhones == null)
            {
                try
                {
                    User user = db.Users.Find(userId);
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ReturningUser", new { msg = "Error Encountered:<br />" + ex.Message });
                }
            }

            return RedirectToAction("Cancel", "Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser([Bind(Include = "UserId,NewPassword,OldPassword,FirstName,LastName,Email,Address,Address2,City,State,Zip,PhoneNumber")] User user, string OldPassword = null, string NewPassword = null)
        {
            // Validate Old Password
            bool passwordsMatch = false;
            if (!String.IsNullOrEmpty(NewPassword))
            {
                User validateUser = db.Users.Find(int.Parse(user.UserId.ToString()));
                if (validateUser.Password == OldPassword)
                {
                    passwordsMatch = true;
                }
                else
                {
                    string errMsg = "Password Not Changed - Old Password Incorrect";
                    return RedirectToAction("UpdateReturningUser", "Users", new { errMsg });
                }
            }

            // Validate Mailing Address
            if (Request.Form["address"] != null)
            {
                MailController mail = new MailController();
                if (!mail.ValidateAddress(Request.Form["address"], Request.Form["city"], Request.Form["state"]))
                {
                    return RedirectToAction("UpdateReturningUser", "Users", new { msg = "USPS Validation - Mailing Address Not Found" });
                }
            }

            try
            {
                // Update User Information
                var updateUser = new User()
                {
                    UserId = int.Parse(user.UserId.ToString())
                            ,
                    Password = NewPassword
                            ,
                    FirstName = user.FirstName
                            ,
                    LastName = user.LastName
                            ,
                    Email = user.Email
                            ,
                    Address = user.Address
                            ,
                    Address2 = user.Address2
                            ,
                    City = user.City
                            ,
                    State = user.State
                            ,
                    Zip = user.Zip
                            ,
                    LastLogin = DateTime.UtcNow
                };

                using (var db = new CellableEntities())
                {
                    db.Users.Attach(updateUser);
                    if (!String.IsNullOrEmpty(NewPassword) && passwordsMatch)
                    {
                        db.Entry(updateUser).Property(x => x.Password).IsModified = true;
                    }
                    db.Entry(updateUser).Property(x => x.FirstName).IsModified = true;
                    db.Entry(updateUser).Property(x => x.LastName).IsModified = true;
                    db.Entry(updateUser).Property(x => x.Email).IsModified = true;
                    db.Entry(updateUser).Property(x => x.Address).IsModified = true;
                    db.Entry(updateUser).Property(x => x.Address2).IsModified = true;
                    db.Entry(updateUser).Property(x => x.City).IsModified = true;
                    db.Entry(updateUser).Property(x => x.State).IsModified = true;
                    db.Entry(updateUser).Property(x => x.Zip).IsModified = true;
                    db.Entry(updateUser).Property(x => x.LastLogin).IsModified = true;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                }

                return RedirectToAction("ReturningUser");
            }
            catch (Exception ex)
            {
                ViewBag.Message("Error encountered while updating user:<br />" + ex.Message);
                return RedirectToAction("UpdateReturningUser");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(User user, string UserExists = null)
        {
            // Insert Additional Data into Model
            user.CreatedBy = "System";
            user.CreateDate = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow;
            user.PermissionId = 2;
            user.ConfirmPassword = user.Password;

            // Validate Mailing Address
            if (Request.Form["address"] != null)
            {
                MailController mail = new MailController();
                if (!mail.ValidateAddress(Request.Form["address"], Request.Form["city"], Request.Form["state"]))
                {
                    return RedirectToAction("Register", new { msg = "USPS Validation - Mailing Address Not Found" });
                }
            }

            try
            {
                if (UserExists == null)
                {
                    // Save User Information
                    db.Users.Add(user);
                    db.SaveChanges();
                    Session["LoggedInUserId"] = user.UserId;
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Register", new { msg = "Error Creating User - " + ex.Message });
            }

            return RedirectToAction("ReturningUser"); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "UserId,UserName,Password,PaymentTypes,FirstName,LastName,Email,Address,Address2,City,State,Zip,PhoneNumber")] User user, string UserExists = null, string userEmail = null)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Get Payment User Name & Payment Type
                    string paymentUserName = Request.Form["PaymentUserName"];
                    int paymentTypeId = int.Parse(Request.Form["PaymentTypes"].ToString());

                    // Set User Name Session Variables
                    User sessionUser = db.Users.Find(int.Parse(Session["LoggedInUserId"].ToString()));
                    var userId = sessionUser.UserId;

                    // Set User Name Session Variables
                    Session["LoggedInUser"] = sessionUser.UserName;
                    Session["LoggedInUserId"] = sessionUser.UserId;
                    loggedInUser.UserId = sessionUser.UserId;
                    loggedInUser.UserName = sessionUser.UserName;

                    // Save User Phone
                    UserPhone userPhone = new UserPhone();
                    userPhone.UserId = sessionUser.UserId;
                    userPhone.PhoneId = int.Parse(Session["PhoneBrand"].ToString());
                    userPhone.CarrierId = int.Parse(Session["Carrier"].ToString());
                    userPhone.VersionId = int.Parse(Session["VersionId"].ToString());
                    userPhone.CreateDate = DateTime.Now;
                    db.UserPhones.Add(userPhone);
                    db.SaveChanges();
                    var userPhoneId = userPhone.UserPhoneId;

                    var i = 0;
                    // Save User Answers
                    foreach (var item in Session)
                    {
                        if (item.ToString().Contains("QuestionAnswer"))
                        {
                            UserAnswer userAnswer = new UserAnswer();

                            string groupAnswerSession = Session["QuestionAnswer" + i].ToString();
                            string[] delimiter = new string[] { "_" };
                            string[] groupAnswer;
                            groupAnswer = groupAnswerSession.Split(delimiter, StringSplitOptions.None);

                            userAnswer.Answer = true;
                            userAnswer.PossibleDefectId = int.Parse(groupAnswer[1]);
                            userAnswer.UserPhoneId = userPhoneId;
                            userAnswer.DefectGroupId = int.Parse(groupAnswer[0].ToString());
                            userAnswer.UserAnswerId = int.Parse(groupAnswer[1].ToString());

                            db.UserAnswers.Add(userAnswer);
                            db.SaveChanges();
                        }
                    }

                    // Create Order
                    Order order = new Order();
                    order.Amount = decimal.Parse(Session["Phone Value"].ToString());
                    order.UserId = sessionUser.UserId;
                    order.OrderStatusId = 1;
                    if (Session["PromoCode"] != null)
                    {
                        order.PromoId = int.Parse(Session["PromoCodeId"].ToString());
                    }
                    else
                    {
                        order.PromoId = null;
                    }
                    order.UserPhoneId = userPhoneId;
                    order.PaymentTypeId = paymentTypeId;
                    order.PaymentUserName = paymentUserName;
                    order.CreateDate = DateTime.Now;
                    order.CreateBy = "System";
                    db.Orders.Add(order);
                    db.SaveChanges();
                    var orderId = order.OrderID;
                    Session["OrderId"] = orderId;

                    // Get the Version Info for this Particular Phone
                    PhoneVersion phoneVersion = db.PhoneVersions.Find(int.Parse(Session["VersionId"].ToString()));

                    // Update Phone Version View Count to DB
                    if (phoneVersion.Purchases == null)
                    {
                        phoneVersion.Purchases = 1;
                    }
                    else
                    {
                        phoneVersion.Purchases += 1;
                    }
                    db.Entry(phoneVersion).State = EntityState.Modified;
                    db.SaveChanges();

                    dbContextTransaction.Commit();

                    // Get/Save Shipping Label
                    MailController mail = new MailController();
                    mail.GetShippingLabel(userId, orderId);

                    // Send Confirmation Email(s)
                    EmailController email = new EmailController();
                    email.SendEmail(orderId, "Confirm", userEmail);


                    return RedirectToAction("TrackOrders", "Users", new { NewOrder = "true" });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    ViewBag.Message = "An error was encountered while attempting to complete your transaction. " +
                        "If the error continues, please contact us directly.<br />" +
                        ex.Message;
                    ViewBag.PaymentTypes = new SelectList(db.PaymentTypes, "PaymentTypeId", "PaymentType1", "-- How You Get Paid --");
                    ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", "-- Select State --");

                    return View("Register");
                }
            }
        }

        public ActionResult TrackOrders()
        {
            int userId = int.Parse(Session["LoggedInUserId"].ToString());

            List<vmOrderDetails> orderDetailsVMlist = new List<vmOrderDetails>();

            // Refer to DB/GetOrderDetails.sql
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
                           where o.UserId == userId && up.UserPhoneId == o.UserPhoneId

                           select new vmOrderDetails()
                           {
                               OrderId = o.OrderID,
                               Amount = o.Amount,
                               Brand = ph.Brand,
                               Version = pv.Version,
                               StatusType = os.StatusType,
                               PromoCode = p.PromoCode,
                               PromoName = p.PromoName,
                               Discount = p.Discount,
                               PaymentType = pt.PaymentType1,
                               PaymentUserName = o.PaymentUserName,
                               TrackingNumber = o.USPSTrackingId,
                               MailLabel = o.MailingLabel,
                               CreateDate = o.CreateDate
                           }).ToList();


            foreach (var item in results)
            {
                vmOrderDetails vmDetails = new vmOrderDetails();

                vmDetails.OrderId = item.OrderId;
                vmDetails.Amount = item.Amount;
                vmDetails.Brand = item.Brand;
                vmDetails.Version = item.Version;
                vmDetails.StatusType = item.StatusType;
                vmDetails.PromoCode = item.PromoCode;
                vmDetails.PromoName = item.PromoName;
                vmDetails.Discount = item.Discount;
                vmDetails.PaymentType = item.PaymentType;
                vmDetails.PaymentUserName = item.PaymentUserName;
                vmDetails.MailLabel = item.MailLabel;
                vmDetails.TrackingNumber = item.TrackingNumber;
                vmDetails.CreateDate = item.CreateDate;
                orderDetailsVMlist.Add(vmDetails);
            }

            ViewBag.TrackingInfo = MailHelper.TrackingMessage;

            var title = db.SystemSettings.Find(33);
            var text = db.SystemSettings.Find(34);
            var footer = db.SystemSettings.Find(35);

            ViewBag.Title = title.Value;
            ViewBag.Text = text.Value;
            ViewBag.Footer = footer.Value;

            return View(orderDetailsVMlist);
        }

        public ActionResult SaveTestimonial()
        {
            if (Request["Stars"] != "")
            {
                Testimonial testimonial = new Testimonial();
                testimonial.Rating = int.Parse(Request["Stars"].ToString());
                testimonial.Text = Request["Comment"];
                testimonial.CreateDate = DateTime.Now;
                testimonial.UserId = int.Parse(Session["LoggedInUserId"].ToString());
                db.Testimonials.Add(testimonial);
                db.SaveChanges();
            }

            return RedirectToAction("TrackOrders");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult ResetPassword(string Email, string NewPassword)
        {
            // Validate User Email
            var userRec = db.Users.SingleOrDefault(x => x.Email == Email);

            if (userRec != null)
            {
                // Find User
                User user = db.Users.Find(userRec.UserId);

                try
                {
                    if (!String.IsNullOrEmpty(NewPassword))
                    {
                        // Update Password
                        user.Password = NewPassword;
                        db.Entry(user).Property(x => x.Password).IsModified = true;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();

                        // Delete Remember Me Cookie
                        Response.Cookies["UserCookie"].Expires = DateTime.Now.AddDays(-1);

                        // Redirect to Login Page
                        return RedirectToAction("Login", "Users", new { resetSuccess = true });
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ForgotPassword", new { email = Email, msg = "Error encountered while updating user password:<br />" + ex.Message });
                }
            }

            return RedirectToAction("ForgotPassword", new { email = Email, msg = "User Not Found" });
        }

        public ActionResult Login(bool resetPassword = false, bool resetSuccess = false)
        {
            if (resetPassword)
            {
                EmailController email = new EmailController();
                email.SendEmail(null, "Password", Request["Email"]);

                ViewBag.Message = "A password reset email has been sent to your inbox.";
            }

            if (resetSuccess)
            {
                ViewBag.Message = "Your password has been successfully rest.";
            }

            HttpCookie userCookie = Request.Cookies["UserCookie"];
            if (userCookie != null)
            {
                ViewBag.UserName = Request.Cookies["UserCookie"]["UserName"];
            }

            return View();
        }

        public ActionResult Cancel()
        {
            // Remove All Session Variables
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
    }
}
