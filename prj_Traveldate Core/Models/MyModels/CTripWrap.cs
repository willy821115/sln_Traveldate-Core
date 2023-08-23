namespace prj_Traveldate_Core.Models.MyModels
{
    public class CTripWrap
    {
        private Trip _trip = null;

        public CTripWrap() 
        {
        _trip = new Trip();
        }

        public Trip trip { get { return _trip;}set { _trip = value; } }

        public int TripId 
        {
            get {return _trip.TripId; }
            set { _trip.TripId = value; }
         }

        public int ProductId
        {
            get { return _trip.ProductId; }
            set { _trip.ProductId = value; }
        }

        public DateTime? Date
        {
            get { return _trip.Date; }
            set { _trip.Date = value; }
        }

        public decimal? UnitPrice
        {
            get { return _trip.UnitPrice; }
            set { _trip.UnitPrice = value; }
        }

        public int? MinNum
        {
            get { return _trip.MinNum; }
            set { _trip.MinNum = value; }
        }
        public int? MaxNum
        {
            get { return _trip.MaxNum; }
            set { _trip.MaxNum = value; }
        }
        public decimal? Discount
        {
            get { return _trip.Discount; }
            set { _trip.Discount = value; }
        }
        public DateTime? DiscountExpirationDate
        {
            get { return _trip.DiscountExpirationDate; }
            set { _trip.DiscountExpirationDate = value; }
        }
        public string productName { get; set; }

        public string productType { get; set; }

        public int stock { get; set; }

        public int day { get; set; }

        public string? date1 { get; set; }

        public string?  tripDates { get; set; }

        public string? discountLimitDate { get; set; }
    }
}
