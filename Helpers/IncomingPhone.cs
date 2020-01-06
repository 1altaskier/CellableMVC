using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CellableMVC.Models;

namespace CellableMVC.Helpers
{
    public class IncomingPhone
    {
        public int? BrandId { get; set; }

        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }

        public int? VersionId { get; set; }

        [Display(Name = "Version Name")]
        public string VersionName { get; set; }

        [Display(Name = "Base Cost")]
        public decimal? BaseCost { get; set; }

        [Display(Name = "Phone Value")]
        public decimal? PhoneValue { get; set; }

        public string ImageLocation { get; set; }

        [Display(Name = "Promo Code")]
        public string PromoCode { get; set; }

        [Display(Name = "Promo Value")]
        public decimal? PromoValue { get; set; }

        [Display(Name = "Phone Defects")]
        IList<PossibleDefect> PossibleDefects { get; set; }

        // THIS PROBABLY NEEDS TO GO!!!!!!!!
        [Display(Name = "Phone Brand Name")]
        public string PhoneBrandName { get; set; }

    }
}