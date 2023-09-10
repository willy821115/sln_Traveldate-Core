using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CpasswordChangeViewModel 
    {
        //金建立 2023.08.20
        public string? txtNewPassword { get; set; }
        public string? txtCheckPassword { get; set; }

        private Member _member = null;
        private LevelList _levelList = null;
        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }
        public LevelList levelList
        {
            get { return _levelList; }
            set { _levelList = value; }
        }
        public int MemberId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public int? LevelId { get; set; }
        public string? Password { get; set; }


        public string? Level { get; set; }
        public string? ImagePath { get; set; }

    }
}
