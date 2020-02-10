using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CellableMVC.Models
{
    public class vmOrderDetails
    {
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string Brand { get; set; }

        public string Version { get; set; }

        [Display(Name = "Status Type")]
        public string StatusType { get; set; }

        [Display(Name = "Promo Code")]
        public string PromoCode { get; set; }

        [Display(Name = "Promo Name")]
        public string PromoName { get; set; }

        public decimal? Discount { get; set; }

        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }

        [Display(Name = "Payment User Name")]
        public string PaymentUserName { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; }

        [Display(Name = "Mailing Label")]
        public string MailLabel { get; set; }

        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; }
    }
}