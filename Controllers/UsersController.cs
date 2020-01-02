﻿using CellableMVC.Models;
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
            ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", "-- Select State --");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "UserId,UserName,Password,FirstName,LastName,Email,Address,Address2,City,State,Zip,PhoneNumber")] User user)
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

                SaveUserPhone(user.UserId);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error encountered while attempting to register user";
                ViewBag.State = new SelectList(db.States, "StateAbbrv", "StateName", "-- Select State --");
                return View("Register");
            }
        }

        public void SaveUserPhone(int userId)
        {
            // Save User Phone
            UserPhone userPhone = new UserPhone();
            userPhone.UserId = userId;
            userPhone.PhoneId = 0;
            userPhone.CarrierId = 0;
            userPhone.PhoneId = 0;
            userPhone.VersionId = 0;
            userPhone.CreateDate = DateTime.Now;
            db.SaveChanges();

            // Save User Answers
            UserAnswer userAnswer = new UserAnswer();


            // Create Order
            Order order = new Order();


            foreach (var item in Session)
            {

            }
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            // Remove All Session Variables
            Session.Clear();

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
