using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellableMVC.Helpers
{
    public class MailHelper
    {
        private static string _trackingMessage = "";

        public static string TrackingMessage
        {
            get
            {
                return _trackingMessage;
            }
            set
            {
                _trackingMessage = value;
            }
        }
    }
}