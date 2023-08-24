using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace prj_Traveldate_Core.Models
{
    public class CMemberModel
    {
        //金建立 2023.08.24
        public int MemberId { get; set; }

        [DisplayName("姓")]
        public string? LastName { get; set; }

        [DisplayName("名")]
        public string? FirstName { get; set; }

        [DisplayName("性別")]
        public string? Gender { get; set; }

        public string? Idnumber { get; set; }

        [DisplayName("出生日期")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public int? CityId { get; set; }

        [DisplayName("手機號碼")]
        public string? Phone { get; set; }

        [DisplayName("聯絡信箱")]
        public string? Email { get; set; }

        public string? Password { get; set; }

        public int? LevelId { get; set; }

        public int? Discount { get; set; }

        public bool? Enable { get; set; }

        public byte[]? Photo { get; set; }

        public string? ImagePath { get; set; }

        public virtual CityList? City { get; set; }

        public virtual ICollection<CommentList> CommentLists { get; set; } = new List<CommentList>();

        public virtual ICollection<Companion> Companions { get; set; } = new List<Companion>();

        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public virtual ICollection<ForumList> ForumLists { get; set; } = new List<ForumList>();

        public virtual LevelList? Level { get; set; }

        public virtual ICollection<LikeList> LikeLists { get; set; } = new List<LikeList>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<ReplyList> ReplyLists { get; set; } = new List<ReplyList>();
    }
}
