using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CfavoriteListViewModel
    {
        private Member _member = null;
        private Favorite _favorite = null;
        private ProductList _productList = null;

        public Member member
        {
         get { return _member; } 
         set { _member = value; } 
        }
        public Favorite favorite
        {
            get { return _favorite; }
            set { _favorite = value; }
        }
        public ProductList productList
        {
            get { return _productList; }
            set { _productList = value; }
        }
        public CfavoriteListViewModel()
        {
            _member=new Member();
            _favorite=new Favorite();
            _productList=new ProductList();
        }
        public string? ProductName
        {
            get { return _productList.Description; }
            set { _productList.Description = value; }
        }
        public string? Description
        {
            get { return _productList.Description; }
            set { _productList.Description = value; }
        }
        public string? Outline
        {
            get { return _productList.Description; }
            set { _productList.Description = value; }
        }
        public int ProductId
        {
            get { return _productList.ProductId; }
            set { _productList.ProductId = value; }
        }


    }
}
