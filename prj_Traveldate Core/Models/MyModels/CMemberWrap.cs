using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prj_Traveldate_Core.Models.MyModels
{
    public class CMemberWrap
    {
        private Member _member = null;
        public CMemberWrap()
        {
            _member = new Member();
        }
        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }
        [DisplayName("會員編號")]
        public int MemberId 
        {
            get { return _member.MemberId; }
            set { _member.MemberId = value; }
        }
        [DisplayName("姓氏")]
        [Required(ErrorMessage = "{0}為必填")]
        public string? LastName
        {
            get { return _member.LastName; }
            set { _member.LastName = value; }
        }

        [DisplayName("名字")]
        [Required(ErrorMessage = "{0}為必填")]
        public string? FirstName
        {
            get { return _member.FirstName; }
            set { _member.FirstName = value; }
        }

        [DisplayName("性別")]
        public string? Gender
        {
            get { return _member.Gender; }
            set { _member.Gender = value; }
        }

        [DisplayName("身分證字號")]
        [Required(ErrorMessage = "{0}為必填")]
        public string? Idnumber
        {
            get { return _member.Idnumber; }
            set { _member.Idnumber = value; }
        }

        [DisplayName("出生日期")]
        [Required(ErrorMessage = "{0}為必填")]
        public DateTime? BirthDate
        {
            get { return _member.BirthDate; }
            set { _member.BirthDate = value; }
        }

        [DisplayName("居住地")]
        public int? CityId
        {
            get { return _member.CityId; }
            set { _member.CityId = value; }
        }

        [DisplayName("聯絡電話")]
        [Required(ErrorMessage = "{0}為必填")]
        public string? Phone
        {
            get { return _member.Phone; }
            set { _member.Phone = value; }
        }

        [DisplayName("E-mail")]
        [Required(ErrorMessage = "{0}為必填")]
        [EmailAddress(ErrorMessage = "請填寫正確E-mail")]
        public string? Email
        {
            get { return _member.Email; }
            set { _member.Email = value; }
        }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "{0}為必填")]
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

        public bool? Enable
        {
            get { return _member.Enable; }
            set { _member.Enable = value; }
        }
        public bool? Verified
        {
            get { return _member.Verified; }
            set { _member.Verified = value; }
        }


        public string? ImagePath
        {
            get { return _member.ImagePath; }
            set { _member.ImagePath = value; }
        }

        public IFormFile? photo { get; set; }

    }
}
