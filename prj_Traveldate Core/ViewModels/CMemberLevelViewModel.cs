using prj_Traveldate_Core.Models;
using System.ComponentModel.DataAnnotations;

namespace prj_Traveldate_Core.ViewModels
{
    public class CMemberLevelViewModel
    {
        //金建立 2023.08.20
        public List<Member>? Member { get; set; }
        public List<LevelList>? LevelList { get; set; }

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
        public CMemberLevelViewModel()
        {
            _member = new Member();
            _levelList = new LevelList();
        }

        public int MemberId
        {
            get { return _member.MemberId; }
            set { _member.MemberId = value; }
        }
        public string? LastName
        {
            get { return _member.LastName; }
            set { _member.LastName = value; }
        }
        public string? FirstName
        {
            get { return _member.FirstName; }
            set { _member.FirstName = value; }
        }
        public string? Gender
        {
            get { return _member.Gender; }
            set { _member.Gender = value; }
        }
        public string? Idnumber
        {
            get { return _member.Idnumber; }
            set { _member.Idnumber = value; }
        }
        [DataType(DataType.Date)]
        public DateTime? BirthDate
        {
            get { return _member.BirthDate; }
            set { _member.BirthDate = value; }
        }
        public int? CityId
        {
            get { return _member.CityId; }
            set { _member.CityId = value; }
        }
        public string? Phone
        {
            get { return _member.Phone; }
            set { _member.Phone = value; }
        }
        public string? Email
        {
            get { return _member.Email; }
            set { _member.Email = value; }
        }
        public string? Password
        {
            get { return _member.Password; }
            set { _member.Password = value; }
        }
        public int? LevelId
        {
            get { return _member.LevelId; }
            set { _member.LevelId = value; }
        }
        public int? Discount
        {
            get { return _member.Discount; }
            set { _member.Discount = value; }
        }
        //public bool? Enable
        //{
        //    get { return _member.Enable; }
        //    set { _member.Enable = value; }
        //}
        //public byte[]? Photo
        //{
        //    get { return _member.Photo; }
        //    set { _member.Photo = value; }
        //}
        //public string? ImagePath
        //{
        //    get { return _member.ImagePath; }
        //    set { _member.ImagePath = value; }
        //}


        public string? Level
        {
            get { return _levelList.Level; }
            set { _levelList.Level = value; }
        }

    }
}
