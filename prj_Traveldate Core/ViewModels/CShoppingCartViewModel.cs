using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.ViewModels
{
    public class CShoppingCartViewModel
    {
        public List<CCartItem> cartitems { get; set; }
        public List<ProductList> recommends { get; set; }
    }
}
