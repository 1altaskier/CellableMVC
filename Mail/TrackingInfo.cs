using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellableMVC.Mail
{
    public class TrackingInfo
    {
        public TrackingInfo()
        {
            _Details = new List<string>();
        }

        private string _TrackingNumber;
        /// <summary>
        /// The tracking number for the package
        /// </summary>
        public string TrackingNumber
        {
            get { return _TrackingNumber; }
            set { _TrackingNumber = value; }
        }

        private string _Summary;
        /// <summary>
        /// Summary information for the package
        /// </summary>
        public string Summary
        {
            get { return _Summary; }
            set { _Summary = value; }
        }

        private List<string> _Details;
        /// <summary>
        /// Tracking Details
        /// </summary>
        public List<string> Details
        {
            get { return _Details; }
            set { _Details = value; }
        }

        public static TrackingInfo FromXml(string xml)
        {
            int idx1 = 0;
            int idx2 = 0;
            TrackingInfo t = new TrackingInfo();
            if (xml.Contains("<TrackSummary>"))
            {
                idx1 = xml.IndexOf("<TrackSummary>") + 14;
                idx2 = xml.IndexOf("</TrackSummary>");
                t._Summary = xml.Substring(idx1, idx2 - idx1);
            }
            int lastidx = 0;
            while (xml.IndexOf("<TrackDetail>", lastidx) > -1)
            {
                idx1 = xml.IndexOf("<TrackDetail>", lastidx) + 13;
                idx2 = xml.IndexOf("</TrackDetail>", lastidx + 13);
                t.Details.Add(xml.Substring(idx1, idx2 - idx1));
                lastidx = idx2;
            }
            return t;
        }


    }
}