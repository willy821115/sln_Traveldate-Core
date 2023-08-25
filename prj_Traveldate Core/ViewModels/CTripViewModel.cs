using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.ViewModels
{
    public class CTripViewModel
    {
        public List<CTripWrap> cTripWraps { get; set; }
        public List<CProductWrap> cProductWraps { get; set; }

        public List<ProductTypeList> types { get; set; }

        public List<string> statuses { get; set; }

        public int TripStock(int tripID)
        {
            TraveldateContext _db = new TraveldateContext();

            var q = _db.Trips.Where(s => s.TripId == tripID).Select(s => new { orders = s.OrderDetails.Count, max = s.MaxNum }).FirstOrDefault();
            int stock = (int)q.max - (int)q.orders;
            return stock;
        }
    }
}
