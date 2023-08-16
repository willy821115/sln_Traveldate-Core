using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using X.PagedList;

namespace prj_Traveldate_Core.Controllers
{
    public class SearchController : Controller
    {
        CFilteredProductFactory _products = new CFilteredProductFactory();
        CSearchListViewModel _vm = new CSearchListViewModel();
        
        public SearchController()
        {
            _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
        }

        public IActionResult SearchList(CKeywordViewModel keyword, int? page)
        {
             _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
            if (!string.IsNullOrEmpty(keyword.txtKeyword))
            {
                _vm.filterProducts = _products.qureyFilterProductsInfo().Where(p => p.productName.Contains(keyword.txtKeyword)).ToList();
            }
            _vm.categoryAndTags = _products.qureyFilterCategories();//商品類別&標籤,左邊篩選列
            _vm.countryAndCities = _products.qureyFilterCountry();  //商品國家&縣市,左邊篩選列
            _vm.types = _products.qureyFilterTypes();//商品類型,左邊篩選列

            //int pageSize = 5;
            //int pageNumber = page ?? 1;
            //_vm.pages = new PagedList<CFilteredProductItem>(_vm.filterProducts, pageNumber, pageSize);
            //_vm.pages = new StaticPagedList<CFilteredProductItem>(_vm.filterProducts, pageNumber, pageSize, _vm.filterProducts.Count);
            return View(_vm);
        }
        public IActionResult sortBy(string status)
        {
                if (status == "hot")
                {
                    _vm.filterProducts = _vm.filterProducts.OrderByDescending(p => p.orederCount).ToList();//商品cards;
                    return PartialView(_vm);
                }
                if (status == "comment")
                {
                    _vm.filterProducts = _vm.filterProducts.OrderByDescending(p => p.commentAvgScore).ToList();//商品cards;
                    return PartialView(_vm);
                }
                if (status == "price")
                {
                    _vm.filterProducts = _vm.filterProducts.OrderBy(p => p.price).ToList();//商品cards;
                    return PartialView(_vm);
                }
                return PartialView(_vm);
            }
        }
    }

