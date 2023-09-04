using prj_Traveldate_Core.Models;
using System.ComponentModel.DataAnnotations;

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
        private CommentList _commentList = null;
        private CommentPhotoList _commentPhotoList = null;

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
        public CommentList commentList
        {
            get { return _commentList; }
            set { _commentList = value; }
        }
        public CommentPhotoList commentPhotoList
        {
            get { return _commentPhotoList; }
            set { _commentPhotoList = value; }
        }

        public int OrderId { get; set; }

        [DataType(DataType.Date)]
        public string? Date { get; set; }

        [DataType(DataType.Date)]
        public string? Datetime { get; set; }

        public string? TripDetaill { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }

        public int MemberId { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public int? CommentScore { get; set; }

        public int CommentPhotoListId { get; set; }

        public int? CommentId { get; set; }

        public string? ImagePath { get; set; }

        public List<IFormFile> photos { get; set; }

        public virtual ICollection<COrdersViewModel> OrdersViewModel { get; set; } = new List<COrdersViewModel>();
    }
}
