using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CCouponSendViewModel
    {     
        public List<CouponList> Coupons { get; set; }
        public List<Member> Members { get; set; }
        public int SelectedCouponId { get; set; }
        public List<int> SelectedMemberIds { get; set; }
        public bool SendToAllMembers { get; set; }
        public bool SendToNormalMembers { get; set; }
        public bool SendToSilverMembers { get; set; }
        public bool SendToGoldMembers { get; set; }
        public bool SendToDiamondMembers { get; set; }

        public string email { get; set; }
    }
}
