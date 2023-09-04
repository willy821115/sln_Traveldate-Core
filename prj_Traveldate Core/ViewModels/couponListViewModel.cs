using prj_Traveldate_Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace prj_Traveldate_Core.ViewModels
{
    public class couponListViewModel
    {
        private Coupon _coupon = null;
        private CouponList _couponlist = null;
        private Member _member = null;

        public Coupon coupon
        {
            get { return _coupon; }
            set { _coupon = value; }
        }
        public CouponList couponList
        {
            get { return _couponlist; }
            set { _couponlist = value; }
        }

        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }
        public couponListViewModel()
        {
            _coupon = new Coupon();
            _couponlist = new CouponList();
            _member = new Member();
        }
        public int CouponListId
        {
            get { return _coupon.CouponListId; }
            set { _coupon.CouponListId = value; }
        }
        public string? CouponName
        {
            get { return _couponlist.CouponName; }
            set { _couponlist.CouponName = value; }
        }
        public decimal? Discount
        {
            get { return _couponlist.Discount; }
            set { _couponlist.Discount = value; }
        }
        public string? Description
        {
            get { return _couponlist.Description; }
            set { _couponlist.Description = value; }
        }

       // [DataType(DataType.Date)]
        public DateTime? DueDate
        {
            get { return _couponlist.DueDate; }
            set { _couponlist.DueDate = value; }
        }
        public string? ImagePath { get; set; }
        public List<IFormFile>? photos { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

            public virtual CouponList CouponList { get; set; } = null!;

        public virtual Member Member { get; set; } = null!;
        public virtual ICollection<CommentList> CommentLists { get; set; } = new List<CommentList>();

        public virtual ICollection<Companion> Companions { get; set; } = new List<Companion>();

        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public virtual ICollection<ForumList> ForumLists { get; set; } = new List<ForumList>();

        public virtual LevelList? Level { get; set; }

        public virtual ICollection<LikeList> LikeLists { get; set; } = new List<LikeList>();

        public virtual ICollection<ReplyList> ReplyLists { get; set; } = new List<ReplyList>();
    }
}
