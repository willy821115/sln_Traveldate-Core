using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using X.PagedList;
using System.Text.Json;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Azure;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata.Ecma335;

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
        int pageSize = 5;
        int itemsPerPage = 0; // 每頁顯示的項目數
        int itemsToSkip = 0;

        public IActionResult SearchList(int page = 1)
        {
            try
            {
                _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
                _vm.categoryAndTags = _products.qureyFilterCategories();//商品類別&標籤,左邊篩選列
                _vm.countryAndCities = _products.qureyFilterCountry();  //商品國家&縣市,左邊篩選列
                _vm.types = _products.qureyFilterTypes();//商品類型,左邊篩選列

                    json = JsonSerializer.Serialize(_vm.filterProducts);
                    HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
               
                int currentPage = page < 1 ? 1 : page;
               _vm.pages = _vm.filterProducts.ToPagedList(currentPage, pageSize);
                return View(_vm);
            }
            catch (IOException)
            {
                return RedirectToAction("Error", "Error");
            }
            
        }
        public IActionResult sortBy(string sortType,int page=1)
        {
            ViewBag.SortType = sortType;
            int currentPage = page < 1 ? 1 : page; 
           
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_FILETREDPRODUCTS_INFO))
            {
                _vm.filterProducts = _products.qureyFilterProductsInfo().ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }
            

            try
            {
                json = HttpContext.Session.GetString(CDictionary.SK_FILETREDPRODUCTS_INFO);
                _vm.filterProducts = JsonSerializer.Deserialize<List<CFilteredProductItem>>(json);
                // 在這裡處理成功反序列化的結果
            }
            catch (JsonException ex)
            {
                // 處理 JSON 反序列化失敗的例外，可以記錄錯誤訊息或採取適當的措施
                Console.WriteLine("JSON 反序列化失敗：" + ex.Message);
                return RedirectToAction("Error", "Error");
            }


            if (sortType == "hot")
            {
                _vm.filterProducts = _vm.filterProducts.OrderByDescending(p => p.orederCount).ToList();//商品cards;
                _vm.pages = _vm.filterProducts.ToPagedList(currentPage, pageSize);
                
                return PartialView(_vm);
            }
            if (sortType == "comment")
            {
                _vm.filterProducts = _vm.filterProducts.OrderByDescending(p => p.commentAvgScore).ToList();//商品cards;
                _vm.pages = _vm.filterProducts.ToPagedList(currentPage, pageSize);
                return PartialView(_vm);
            }
            if (sortType == "price")
            {
                _vm.filterProducts = _vm.filterProducts.OrderBy(p => p.price).ToList();//商品cards;
                _vm.pages = _vm.filterProducts.ToPagedList(currentPage, pageSize);
                return PartialView(_vm);
            }
            if (sortType == "stock")
            {
                _vm.filterProducts = _vm.filterProducts.OrderByDescending(p => p.prodStock).ToList();//商品cards;
                _vm.pages = _vm.filterProducts.ToPagedList(currentPage, pageSize);
                return PartialView(_vm);
            }
            _vm.pages = _vm.filterProducts.ToPagedList(currentPage, pageSize);
            return PartialView(_vm);
        }

        public IActionResult filterCity(string? txtKeyword)
        {
            if (!string.IsNullOrEmpty(txtKeyword) && txtKeyword != "undefined")
            {
                var filterCities = _context.ProductLists
                    .Where(p => _products.confirmedId.Contains(Convert.ToInt32( p.ProductId)))
                    .Where(c => c.City.City.Contains(txtKeyword))
                    .GroupBy(p => p.City.City)
                    .Select(g =>
                    new
                    {
                        CityId = g.Select(i => i.CityId),
                        CityName = g.Key.ToString().Trim().Substring(0, 2),
                        countCity = g.Count()
                    }).ToList();
                return Json(filterCities);
            }
            var filterCitiess = _context.ProductLists.Where(p => _products.confirmedId.Contains(Convert.ToInt32( p.ProductId)))
                .GroupBy(p => p.City.City)
                .Select(g => new
                {
                    CityId = g.Select(i => i.CityId),
                    CityName = g.Key.ToString().Trim().Substring(0, 2),
                    countCity = g.Count()
                }).ToList();
            return Json(filterCitiess);
        }

        public IActionResult FilterByConditions(List<string> tags, List<string> cities, List<string> types, string startTime, string endTime, int minPrice, int maxPrice, int page=1)
        {
            _vm.filterProducts = _products.qureyFilterProductsInfo();
            //有篩選條件做篩選
            if (tags.Count > 0)
            {
                _vm.filterProducts = _vm.filterProducts
                              .Where(p => p.productTags.Any(tag => tags.Contains(tag))).ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }
            if (cities.Count > 0)
            {
                _vm.filterProducts = _vm.filterProducts
                                .Where(p => cities.Contains(p.city)).ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }
            if (types.Count > 0)
            {
                _vm.filterProducts = _vm.filterProducts
                               .Where(p => types.Contains(p.type)).ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }
            //沒有篩選條件抓全部
            if (tags.Count == 0 && cities.Count == 0 && types.Count == 0)
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
                _vm.filterProducts = _vm.filterProducts.Where(p => DateTime.Parse(p.date) >= startDateTime && DateTime.Parse(p.date) <= endDateTime).ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
                if (_vm.filterProducts.Count == 0)
                {
                    return Content($"<h4><img src={Url.Content("~/icons/icons8-error-96.png")}>沒有符合篩選的項目</h4><input id={"updateTotal"} type={"hidden"} value={"0"}>");
                }

            }
            //有價格篩選做上面篩選後的篩選
            if (minPrice > 0  && maxPrice>0)
            {
                _vm.filterProducts = _vm.filterProducts.Where(p => p.price >= minPrice && p.price <= maxPrice).ToList();
                json = JsonSerializer.Serialize(_vm.filterProducts);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
                if (_vm.filterProducts.Count == 0)
                {
                    return Content($"<h4><img src={Url.Content("~/icons/icons8-error-96.png")}>沒有符合篩選的項目</h4><input id={"updateTotal"} type={"hidden"} value={"0"}>");
                }
            }

            if (_vm.filterProducts.Count == 0)
            {
                return Content($"<h4><img src={Url.Content("~/icons/icons8-error-96.png")}>沒有符合篩選的項目</h4><input id={"updateTotal"} type={"hidden"} value={"0"}>");
            }
            itemsPerPage = 5; // 每頁顯示的項目數
            itemsToSkip = (page - 1) * itemsPerPage;
            _vm.pageFilterProducts = _vm.filterProducts.Skip(itemsToSkip).Take(itemsPerPage).ToList();
            _vm.currentPage = page;

            return PartialView(_vm);
        }
    }
}   


