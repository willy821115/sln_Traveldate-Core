using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CpasswordChangeViewModel 
    {
        //金建立 2023.08.20
        public string? txtNewPassword { get; set; }
        public string? txtCheckPassword { get; set; }

        private Member _member = null;
        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }
        public int MemberId { get; set; }
    }
}
