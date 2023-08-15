using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CProductDetailViewModel
    {
        public string? productName { get; set; }

        public string? productDate { get; set; }

        public string? Phone { get; set; }

        public int? stock { get; set; }

        public string? productType { get; set; }

        public int TripStock(int tripID) 
        {
        TraveldateContext _db=new TraveldateContext();

            var q = _db.Trips.Where(s => s.TripId == tripID).Select(s =>new {orders= s.OrderDetails.Count, max=s.MaxNum}).FirstOrDefault();
            int stock = (int)q.max - (int)q.orders;
            return stock;
        }
    }
}
