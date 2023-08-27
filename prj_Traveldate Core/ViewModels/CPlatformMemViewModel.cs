using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CPlatformMemViewModel
    {
        private Member _member = null;
        private Company _company = null;

        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }
        public Company company
        {
            get { return _company; }
            set { _company = value; }
        }
        public CPlatformMemViewModel()
        {
            _member = new Member();
            _company = new Company();
        }

        public int MemberId
        {
            get { return member.MemberId; }
            set { member.MemberId = value; }
        }
        public string LastName
        {
            get { return member.LastName; }
            set { member.LastName= value; }
        }
        public string FirstName
        {
            get { return member.FirstName; }
            set { member.FirstName = value; }
        }
        public string Gender
        {
            get { return member.Gender; }
            set { member.Gender = value; }
        }
        public string Idnumber
        {
            get { return member.Idnumber; }
            set { member.Idnumber = value; }
        }
        public DateTime? BirthDate
        {
            get { return member.BirthDate; }
            set { member.BirthDate = value; }
        }
        public string Phone
        {
            get { return member.Phone; }
            set { member.Phone = value; }
        }
        public string Email
        {
            get { return member.Email; }
            set { member.Email = value; }
        }
        public int? Discount
        {
            get { return member.Discount; }
            set { member.Discount = value; }
        }
        public int? LevelId
        {
            get { return member.LevelId; }
            set { member.LevelId = value; }
        }
        public int? CityId
        {
            get { return member.CityId; }
            set { member.CityId = value; }
        }

        public int CompanyId
        {
            get { return company.CompanyId; }
            set { company.CompanyId = value; }
        }
        public string TaxIdNumber
        {
            get { return company.TaxIdNumber; }
            set { company.TaxIdNumber = value; }
        }
        public string CompanyName
        {
            get { return company.CompanyName; }
            set { company.CompanyName = value; }
        }
        public string City
        {
            get { return company.City; }
            set { company.City = value; }
        }
        public string Address
        {
            get { return company.Address; }
            set { company.Address = value; }
        }
        public string CompanyPhone
        {
            get { return company.Phone; }
            set { company.Phone = value; }
        }
        public string Principal
        {
            get { return company.Principal; }
            set { company.Principal = value; }
        }
        public string Contact
        {
            get { return company.Contact; }
            set { company.Contact = value; }
        }
        public string Title
        {
            get { return company.Title; }
            set { company.Title = value; }
        }
        public string ComEmail
        {
            get { return company.Email; }
            set { company.Email = value; }
        }
        public string ServerDescription
        {
            get { return company.ServerDescription; }
            set { company.ServerDescription = value; }
        }
        public bool? Enable
        {
            get { return member.Enable; }
            set { member.Enable = value; }
        }
        public bool? ComEnable
        {
            get { return company.Enable; }
            set { company.Enable = value; }
        }
    }
}
