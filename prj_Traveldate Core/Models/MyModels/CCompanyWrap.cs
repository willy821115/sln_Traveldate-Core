namespace prj_Traveldate_Core.Models.MyModels
{
    public class CCompanyWrap
    {
        private Company _company = null;

        public CCompanyWrap() 
        {
        _company = new Company();
        }

        public Company? company { get { return _company; } set { _company = value; } }

        public int CompanyId
        { 
        get { return _company.CompanyId; } set { _company.CompanyId = value;}
        }

        public string? TaxIdNumber 
        {
            get { return _company.TaxIdNumber; }set { _company.TaxIdNumber = value;}
        }

        public string? CompanyName 
        {
            get { return _company.CompanyName; }set { _company.CompanyName = value;}
        }

        public string? Country 
        {
            get { return _company.Country; } set { _company.Country = value; }
        }

        public string? City 
        {
            get { return _company.City; }set { _company.City = value; }
        }

        public string? PostalCode { get { return _company.PostalCode; }set { _company.PostalCode = value; } }

        public string? Address { get { return _company.Address; }set { _company.Address = value; } }

        public string? Phone { get { return _company.Phone; }set { _company.Phone = value; } }

        public string? Url
        {
            get { return _company.Url; }
            set { _company.Url = value; }
        }

        public string? Principal {
            get { return _company.Principal; }
            set { _company.Principal = value; } }

        public string? Contact { get { return _company.Contact; } set { _company.Contact = value; } }

        public string? Title { get {return  _company.Title; } set { _company.Title = value; } }

        public string? Email { get { return _company.Email; } set { _company.Email = value; } }

        public string? Password { get { return _company.Password; } set { _company.Password = value; } }

        public string? ServerDescription { get { return _company.ServerDescription; } set { _company.ServerDescription = value; } }

        public bool? Enable { get { return _company.Enable; } set { _company.Enable = value; } }

        public List<CityList> cities { get; set; }

        public List<string> country { get; set; }

        public string newPasswordCheck { get; set; }
    }
}
