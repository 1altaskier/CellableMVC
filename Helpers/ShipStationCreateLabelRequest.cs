
namespace shipstation_erp.Models
{
    public class ShipStationCreateLabelRequest : InternationalOptions
    {
        private long? _orderId;
        private string _carrierCode;
        private string _serviceCode;
        private string _packageCode;
        private string _confirmation;
        private string _shipDate;
        private weight _weight;
        private bool _testLabel;

        public long? orderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

        public string carrierCode
        {
            get { return _carrierCode; }
            set { _carrierCode = value; }
        }
        public string serviceCode
        {
            get { return _serviceCode; }
            set { _serviceCode = value; }
        }
        public string packageCode
        {
            get { return _packageCode; }
            set { _packageCode = value; }
        }
        public string confirmation
        {
            get { return _confirmation; }
            set { _confirmation = value; }
        }
        public string shipDate
        {
            get { return _shipDate; }
            set { _shipDate = value; }
        }

        public weight weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public bool testLabel
        {
            get { return _testLabel; }
            set { _testLabel = value; }
        }
    }

    public class shipTo
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

    public class shipFrom
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

    public class weight
    {
        private int _value;
        private string _units;

        public int value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string units
        {
            get { return _units; }
            set { _units = value; }
        }
    }
}
