using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RemoteValiDationInMVC.Models
{
    public class RegisterModel
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Enter EmailId")]
        //Using Remote validation attribute   
        [Remote("IsAlreadySigned", "Register", HttpMethod = "POST", ErrorMessage = "Email already exists in database.")]
        public string UserEmailId { get; set; }
        [Required]
        public string Designation { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = ("Choose Password"))]
        public string PassWord { get; set; }

    }
}