using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using X.PagedList;

namespace prj_Traveldate_Core.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult SearchList(CKeywordViewModel keyword, int? page)
        {
            CFilteredProductFactory products = new CFilteredProductFactory();
            CSearchListViewModel vm = new CSearchListViewModel();
            vm.filterProducts = products.qureyFilterProductsInfo().ToList();//商品cards;
            if (!string.IsNullOrEmpty(keyword.txtKeyword))
            {
                vm.filterProducts = products.qureyFilterProductsInfo().Where(p => p.productName.Contains(keyword.txtKeyword)).ToList();
            }
            vm.categoryAndTags = products.qureyFilterCategories();//商品類別&標籤,左邊篩選列
            vm.countryAndCities = products.qureyFilterCountry();  //商品國家&縣市,左邊篩選列
            vm.types = products.qureyFilterTypes();//商品類型,左邊篩選列

            int pageSize = 5;
            int pageNumber = page ?? 1;
            //vm.pages = new PagedList<CFilteredProductItem>(vm.filterProducts, pageNumber, pageSize);
            vm.pages = new StaticPagedList<CFilteredProductItem>(vm.filterProducts, pageNumber, pageSize, vm.filterProducts.Count);
            return View(vm);
        }
    }
}
