using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CCouponSendViewModel
    {     
        public List<CouponList> Coupons { get; set; }
        public List<Member> Members { get; set; }
        public int SelectedCouponId { get; set; }
        public List<int> SelectedMemberIds { get; set; }

    }
}
