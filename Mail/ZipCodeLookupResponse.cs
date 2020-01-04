using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CellableMVC.Mail
{
    public class AddressValidateResponse
    {
        private AddressValidateResponse()
        {

        }

        private List<Address> _Addresses;
        /// <summary>
        /// A collection of Addresses return from the Address Validation routine.
        /// </summary>
        public List<Address> Addresses
        {
            get { return _Addresses; }
            set { _Addresses = value; }
        }
    }
}