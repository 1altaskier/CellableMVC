using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CellableMVC.Helpers
{
    public class LoggedInUser
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }

        public SelectList StorageCapacities { get; set; }
    }
}