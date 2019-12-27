using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CellableMVC.Controllers
{
    public class PhonesController : Controller
    {
        private CellableEntities db = new CellableEntities();

        // GET: Phones
        public ActionResult Phones()
        {
            var phones = db.Phones.ToList();

            return View(phones);
        }
    }
}