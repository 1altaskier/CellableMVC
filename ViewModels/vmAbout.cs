using CellableMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellableMVC.ViewModels
{
    public class vmAbout
    {
        public string title { get; set; }
        public string text { get; set; }
        public string body { get; set; }
        public string footer { get; set; }
        public string image { get; set; }
        public IList<Testimonial> testimonials { get; set; }
    }
}