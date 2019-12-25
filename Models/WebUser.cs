using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CellableMVC.Models
{
    public class WebUser {  
        // To Check User Name   
        [Remote("UserNameExists", "Validation", ErrorMessage = "User name already exists. Do you already have an account?")]  
        public string UserName
        { get; set; }

        // Check User Name And Email   
        [Remote("UserNameAndEmailExists", "Validation", ErrorMessage = "EmailId is already exists. Do you already have an account?", AdditionalFields = "UserName")]  
        public string UserEmail 
        { get; set; }
    }
}