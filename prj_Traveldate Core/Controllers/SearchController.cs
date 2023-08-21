using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using X.PagedList;
using System.Text.Json;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace prj_Traveldate_Core.Controllers
{
    public class SearchController : Controller
    {
        string? json = null;
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

            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_FILETREDPRODUCTS_INFO))
            {
                 json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }
            //int pageSize = 5;
            //int pageNumber = page ?? 1;
            //_vm.pages = new PagedList<CFilteredProductItem>(_vm.filterProducts, pageNumber, pageSize);
            //_vm.pages = new StaticPagedList<CFilteredProductItem>(_vm.filterProducts, pageNumber, pageSize, _vm.filterProducts.Count);
            return View(_vm);
        }
        public IActionResult sortBy(string sortType)
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_FILETREDPRODUCTS_INFO))
            {
                _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }
            json = HttpContext.Session.GetString(CDictionary.SK_FILETREDPRODUCTS_INFO);
            _vm.filterProducts = JsonSerializer.Deserialize<List<CFilteredProductItem>>(json);

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
        
        public IActionResult FilterByConditions(List<string> conditions , string startTime, string endTime)
        {
            //有篩選條件做篩選
            if (conditions.Count >0)
            {
                _vm.filterProducts =_products.qureyFilterProductsInfo()
                                .Where(p => p.productTags.Any(tag => conditions.Contains(tag))
                                || conditions.Contains(p.city)
                                ).ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }
            //沒有篩選條件抓全部
            else
            {
                _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }
            //有時間條件做上面篩選後的篩選
            if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                DateTime startDateTime = DateTime.Parse(startTime);
                DateTime endDateTime = DateTime.Parse(endTime).AddDays(1);
                _vm.filterProducts = _vm.filterProducts.Where(p => DateTime.Parse(p.date) > startDateTime && DateTime.Parse(p.date) < endDateTime).ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
                return PartialView(_vm);
            }
          
           //1.回傳只有城市/標籤的篩選
           //2.回傳只有時間的篩選
           //3.回傳有時間/標籤/城市的篩選
           //4.回傳沒有任何篩選的結果
            return PartialView(_vm);
        }
        //////////////////20230821跟其他篩選項目整合了////////////////////////////
        //public IActionResult FilterByDate(string startTime, string endTime)
        //{
        //    if (!HttpContext.Session.Keys.Contains(CDictionary.SK_FILETREDPRODUCTS_INFO))
        //    {
        //        _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
        //        json = JsonSerializer.Serialize(_vm.filterProducts);
        //        HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
        //    }

        //    json = HttpContext.Session.GetString(CDictionary.SK_FILETREDPRODUCTS_INFO);
        //    _vm.filterProducts = JsonSerializer.Deserialize<List<CFilteredProductItem>>(json);
        //    if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
        //    {
        //        DateTime startDateTime = DateTime.Parse(startTime);
        //        DateTime endDateTime = DateTime.Parse(endTime).AddDays(1);
        //        _vm.filterProducts = _vm.filterProducts.Where(p => DateTime.Parse(p.date) > startDateTime && DateTime.Parse(p.date) < endDateTime).ToList();
        //        json = JsonSerializer.Serialize(_vm.filterProducts);
        //        HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
        //        return PartialView(_vm);
        //    }
        //    //json = JsonSerializer.Serialize(_vm.filterProducts);
        //    //HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
        //    return PartialView(_vm);
        //}
    }
}

