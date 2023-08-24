using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CConfirmOrderViewModel
    {
        public Member member { get; set; }
        public List<Companion> companions { get; set; }
        public List<CouponList> coupons { get; set; }
        public List<OrderDetail> orders { get;  set; }
    }
}
