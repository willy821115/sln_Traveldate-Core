using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class COrdersViewModel
    {
        private TripDetail _tripDetail = null;
        private Trip _trip = null;
        private OrderDetail _orderDetail = null;
        private Order _order = null;
        private Member _member = null;
        private ProductList _productList = null;

        public TripDetail tripDetai
        {
            get { return _tripDetail; }
            set { _tripDetail = value; }
        }
        public Trip trip
        {
            get { return _trip; }
            set { _trip = value; }
        }
        public OrderDetail orderDetail
        {
            get { return _orderDetail; }
            set { _orderDetail = value; }
        }
        public Order order
        {
            get { return _order; }
            set { _order = value; }
        }

        public Member member
        {
            get { return _member; }
            set { _member = value; }
        }
        public ProductList productList
        {
            get { return _productList; }
            set { _productList = value; }
        }
        public int OrderId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Datetime { get; set; }
        public string? TripDetaill { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
    }
}
