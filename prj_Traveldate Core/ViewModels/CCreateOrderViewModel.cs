using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CCreateOrderViewModel
    {
        public decimal checkoutAmount { get; set; } 
        public Order newOrder { get; set; }
        public List<OrderDetail> ods { get; set; }
        public List<List<CompanionList>> companionLists { get; set; }
        public List<List<Companion>> companions { get; set; }
    }
}
