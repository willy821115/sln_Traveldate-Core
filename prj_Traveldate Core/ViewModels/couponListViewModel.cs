using prj_Traveldate_Core.Models;

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
        public DateTime? DueDate
        {
            get { return _couponlist.DueDate; }
            set { _couponlist.DueDate = value; }
        }
    }
}
