namespace prj_Traveldate_Core.Models.MyModels
{
    public class CCouponWrap
    {
        private CouponList _coupon = null;

        public CCouponWrap()
        {
            _coupon = new CouponList();
        }
        public CouponList CouponList { get { return _coupon; } set { _coupon = value; } }

        public int CouponListId
        {
            get { return _coupon.CouponListId; }
            set { _coupon.CouponListId = value; }
        }
        public string? CouponName
        {
            get { return _coupon.CouponName; }
            set { _coupon.CouponName = value; }
        }
        public decimal? Discount
        {
            get { return _coupon.Discount; }
            set { _coupon.Discount = value; }
        }
        public string? Description
        {
            get { return _coupon.Description; }
            set { _coupon.Description = value; }
        }
        public DateTime? DueDate
        {
            get { return _coupon.DueDate; }
            set { _coupon.DueDate = value; }
        }

        public DateTime? CreateDate
        {
            get { return _coupon.CreateDate; }
            set { _coupon.CreateDate = value; }
        }


        public string? ImagePath
        {
            get { return _coupon.ImagePath; }
            set { _coupon.ImagePath = value; }
        }


        public IFormFile? photo { get; set; }
        public int CouponNum { get; set; }
        public int CouponUsedNum { get; set; }

    }
}
