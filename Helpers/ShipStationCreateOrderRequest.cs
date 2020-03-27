using System.Collections.Generic;

namespace shipstation_erp.Models
{
    public class ShipStationCreateOrderRequest : InternationalOptions
    {
        private string _orderNumber;
        private string _orderDate;
        private string _orderStatus;
        private billTo _billTo;
        private shipTo _shipTo;

        public string orderNumber
        {
            get { return _orderNumber; }
            set { _orderNumber = value; }
        }

        public string orderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; }
        }

        public string orderStatus
        {
            get { return _orderStatus; }
            set { _orderStatus = value; }
        }
        public billTo billTo
        {
            get { return _billTo; }
            set { _billTo = value; }
        }
        public shipTo shipTo
        {
            get { return _shipTo; }
            set { _shipTo = value; }
        }
    }

    public class billTo
    {
        private string _name;
        private string _company;
        private string _street1;
        private string _street2;
        private string _street3;
        private string _city;
        private string _state;
        private string _postalCode;
        private string _country;
        private string _phone;
        private bool _residential;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string company
        {
            get { return _company; }
            set { _company = value; }
        }
        public string street1
        {
            get { return _street1; }
            set { _street1 = value; }
        }
        public string street2
        {
            get { return _street2; }
            set { _street2 = value; }
        }
        public string street3
        {
            get { return _street3; }
            set { _street3 = value; }
        }
        public string city
        {
            get { return _city; }
            set { _city = value; }
        }
        public string state
        {
            get { return _state; }
            set { _state = value; }
        }
        public string postalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }
        public string country
        {
            get { return _country; }
            set { _country = value; }
        }
        public string phone
        {
            get { return _phone; }
            set { _phone = value; }
        }
        public bool residential
        {
            get { return _residential; }
            set { _residential = value; }
        }

    }

    public class customsItems
    {
        private string _description;
        private int _quantity;
        private int _value;
        private string _harmonizedTariffCode;
        private string _countryOfOrigin;

        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
        public int quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        public int value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string harmonizedTariffCode
        {
            get { return _harmonizedTariffCode; }
            set { _harmonizedTariffCode = value; }
        }
        public string countryOfOrigin
        {
            get { return _countryOfOrigin; }
            set { _countryOfOrigin = value; }
        }
    }


    public class oShipInternational
    {
        private string _contents;
        private string _nonDelivery;
        private List<customsItems> _customsItems;

        public string contents
        {
            get { return _contents; }
            set { _contents = value; }
        }
        public string nonDelivery
        {
            get { return _nonDelivery; }
            set { _nonDelivery = value; }
        }
        public List<customsItems> customsItems
        {
            get { return _customsItems; }
            set { _customsItems = value; }
        }
    }
    public class InternationalOptions
    {
        private oShipInternational _internationalOptions;
        public oShipInternational internationalOptions
        {
            get { return _internationalOptions; }
            set { _internationalOptions = value; }
        }
    }
}
