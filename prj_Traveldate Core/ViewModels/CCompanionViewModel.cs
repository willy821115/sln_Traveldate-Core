using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CCompanionViewModel
    {
        private Member _member = null;
        private Companion _companiont = null;

        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }
        public Companion companiont
        {
            get { return _companiont; }
            set { _companiont = value; }
        }
        public CCompanionViewModel()
        {
            _member = new Member();
            _companiont = new Companion();
        }
        public int CompanionId { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? Idnumber { get; set; }

        public string? Phone { get; set; }

        public int? MemberId { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
