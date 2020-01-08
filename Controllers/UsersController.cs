using CellableMVC.Mail;
using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CellableMVC.Helpers;
using System.Data.SqlClient;
using System.Data;

namespace CellableMVC.Controllers
{
    public class UsersController : Controller
    {
        private string USPSAPIUserName = WebConfigurationManager.AppSettings["USPSAPIUserName"];
        private string USPSAPIPassword = WebConfigurationManager.AppSettings["USPSAPIPassword"];
        private string AdminEmail = WebConfigurationManager.AppSettings["AdminEmail"];

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
        public ActionResult Register()
        {
            if (Session["LoggedInUser"] != null)
            {
                return RedirectToAction("ReturningUser");
            }
            ViewBag.PaymentTypes = new SelectList(db.PaymentTypes, "PaymentTypeId", "PaymentType1", "-- How You Get Paid --");
            ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", "-- Select State --");

            return View();
        }

        public ActionResult ReturningUser()
        {
            IList<PossibleDefect> defects = db.PossibleDefects.ToList();
            ViewBag.PossibleDefects = defects;

            ViewBag.PaymentTypes = new SelectList(db.PaymentTypes, "PaymentTypeId", "PaymentType1", "-- How You Get Paid --");

            User user = db.Users.Find(int.Parse(Session["LoggedInUserId"].ToString()));

            return View(user);
        }

        public ActionResult UpdateReturningUser(string errMsg = null)
        {
            LoggedInUser loggedInUser = new LoggedInUser();

            // Get User Info From DB
            User user = db.Users.Find(int.Parse(Session["LoggedInUserId"].ToString()));

            // Find User's Payment Type
            //Order order = db.Orders.FirstOrDefault(x => x.UserId == user.UserId);
            //var paymentType = order.PaymentTypeId;

            // Find User's State
            var state = user.State;

            // Create Drop Down List(s)
            //ViewBag.PaymentTypes = new SelectList(db.PaymentTypes, "PaymentTypeId", "PaymentType1", paymentType);
            ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", state);

            ViewBag.Message = errMsg;

            return View(user);
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
        public ActionResult Register([Bind(Include = "UserId,UserName,Password,PaymentTypes,FirstName,LastName,Email,Address,Address2,City,State,Zip,PhoneNumber")] User user, string UserExists = null)
        {
            // Insert Additional Data into Model
            user.CreatedBy = "System";
            user.CreateDate = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow;
            user.PermissionId = 2;
            user.ConfirmPassword = user.Password;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (UserExists == null)
                    {
                        // Save User Information
                        db.Users.Add(user);
                        db.SaveChanges();
                        Session["LoggedInUserId"] = user.UserId;
                    }

                    // Get Payment User Name & Payment Type
                    string paymentUserName = Request.Form["PaymentUserName"];
                    int paymentTypeId = int.Parse(Request.Form["PaymentTypes"].ToString());

                    // Set User Name Session Variables
                    User sessionUser = db.Users.Find(int.Parse(Session["LoggedInUserId"].ToString()));

                    // Set User Name Session Variables
                    Session["LoggedInUser"] = sessionUser.UserName;
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

                    // Save User Answers
                    foreach (var item in Session)
                    {
                        var temp = 0;
                        if (int.TryParse(item.ToString(), out temp))
                        {
                            if (Session[item.ToString()].ToString() != "0.00" && Session[item.ToString()].ToString() != "0")
                            {
                                UserAnswer userAnswer = new UserAnswer();
                                userAnswer.Answer = true;
                                userAnswer.PossibleDefectId = int.Parse(item.ToString());
                                userAnswer.UserPhoneId = userPhoneId;
                                db.UserAnswers.Add(userAnswer);
                                db.SaveChanges();
                            }
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

                    // Send Confirmation Email(s)
                    EmailController email = new EmailController();
                    email.ConfirmationEmail("REPLACE");

                    dbContextTransaction.Commit();

                    return RedirectToAction("TrackOrders", "Users");
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
                vmDetails.CreateDate = item.CreateDate;
                orderDetailsVMlist.Add(vmDetails);
            }

            return View(orderDetailsVMlist);
        }


        public ActionResult Login()
        {
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
