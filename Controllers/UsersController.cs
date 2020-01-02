using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CellableMVC.Controllers
{
    public class UsersController : Controller
    {
        private CellableEntities db = new CellableEntities();

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
            ViewBag.PaymentTypes = new SelectList(db.PaymentTypes, "PaymentTypeId", "PaymentType1", "-- How You Get Paid --");
            ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", "-- Select State --");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "UserId,UserName,Password,PaymentTypes,FirstName,LastName,Email,Address,Address2,City,State,Zip,PhoneNumber")] User user)
        {
            // Insert Additional Data into Model
            user.CreatedBy = "System";
            user.CreateDate = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow;
            user.PermissionId = 2;
            user.ConfirmPassword = user.Password;

            try
            {
                db.Users.Add(user);
                db.SaveChanges();

                string paymentUserName = Request.Form["PaymentUserName"];
                int paymentTypeId = int.Parse(Request.Form["PaymentTypes"].ToString());

                SaveUserPhone(user.UserId, paymentTypeId, paymentUserName);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error encountered while attempting to register user/phone";
                ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", "-- Select State --");
                return View("Register");
            }
        }

        public void SaveUserPhone(int userId, int paymentTypeId, string paymentUserName)
        {
            // Save User Phone
            UserPhone userPhone = new UserPhone();
            userPhone.UserId = userId;
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
            order.UserId = userId;
            order.OrderStatusId = 1;
            if(Session["PromoCode"] != null)
            {
                order.PromoId = int.Parse(Session["PromoCode"].ToString());
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

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
