using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using X.PagedList;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace prj_Traveldate_Core.Controllers
{
    public class SearchController : Controller
    {
        CFilteredProductFactory _products = new CFilteredProductFactory();
        CSearchListViewModel _vm = new CSearchListViewModel();
        TraveldateContext _context = null;
        public SearchController(TraveldateContext context)
        {
            _context = context;
        }

        public IActionResult SearchList(int? page)
        {
            _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
            _vm.categoryAndTags = _products.qureyFilterCategories();//商品類別&標籤,左邊篩選列
            _vm.countryAndCities = _products.qureyFilterCountry();  //商品國家&縣市,左邊篩選列
            _vm.types = _products.qureyFilterTypes();//商品類型,左邊篩選列

            //int pageSize = 5;
            //int pageNumber = page ?? 1;
            //_vm.pages = new PagedList<CFilteredProductItem>(_vm.filterProducts, pageNumber, pageSize);
            //_vm.pages = new StaticPagedList<CFilteredProductItem>(_vm.filterProducts, pageNumber, pageSize, _vm.filterProducts.Count);
            return View(_vm);
        }
        public IActionResult sortBy(string txtKeyword, string sortType)
        {
            _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
            if (!string.IsNullOrEmpty(txtKeyword))
            {
                _vm.filterProducts = _products.qureyFilterProductsInfo().Where(p => p.productName.Contains(txtKeyword)).ToList();
                return PartialView(_vm);
            }
            if (sortType == "hot")
            {
                _vm.filterProducts = _vm.filterProducts.OrderByDescending(p => p.orederCount).ToList();//商品cards;
                return PartialView(_vm);
            }
            if (sortType == "comment")
            {
                _vm.filterProducts = _vm.filterProducts.OrderByDescending(p => p.commentAvgScore).ToList();//商品cards;
                return PartialView(_vm);
            }
            if (sortType == "price")
            {
                _vm.filterProducts = _vm.filterProducts.OrderBy(p => p.price).ToList();//商品cards;
                return PartialView(_vm);
            }
            return PartialView(_vm);
        }
        
        public IActionResult filterCity(string? txtKeyword)
        {
            if (!string.IsNullOrEmpty(txtKeyword) && txtKeyword != "undefined")
            {
                var filterCities = _context.ProductLists
                    .Where(p=> _products.confirmedId.Contains(p.ProductId))
                    .Where(c=>c.City.City.Contains(txtKeyword))
                    .Select(c=>new { CityId = c.CityId.Value, CityName = c.City.City.Trim().Substring(0,2)}).Distinct().ToList();
                return Json(filterCities);
            }
           var filterCitiess = _context.ProductLists.Where(p => _products.confirmedId.Contains(p.ProductId)).Select(c => new { CityId = c.CityId.Value, CityName = c.City.City.Trim().Substring(0, 2) }).Distinct().ToList();
            return Json(filterCitiess);
        }
        
        public IActionResult FilterByConditions(List<string> conditions)
        { 
            _vm.filterProducts = _products.qureyFilterProductsInfo()
                .Where(p=>p.productTags.Any(tag=> conditions.Contains(tag))
                || conditions.Contains(p.city)
                ).ToList();
            return PartialView(_vm);
        }
    }
}

