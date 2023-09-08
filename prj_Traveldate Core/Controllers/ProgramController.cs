using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace prj_Traveldate_Core.Controllers
{
    public class ProgramController : Controller
    {
        public IActionResult Product(int? id)
        {
            if (id == null)
                return RedirectToAction("SearchList", "Search");

            //將id存入瀏覽紀錄
            List<int> visitedProducts = GetVisitedProductsFromSession();
            visitedProducts.Reverse();
            if (!visitedProducts.Contains((int)id))
            {
                //如果不存在瀏覽紀錄中就直接加
                visitedProducts.Add((int)id);
            }
            else
            {
                //如果存在就換到最新的位置
                visitedProducts.RemoveAt(visitedProducts.IndexOf((int)id));
                visitedProducts.Add((int)id);
            }
            //瀏覽紀錄只存5個商品
            if (visitedProducts.Count > 5)
            {
                visitedProducts = visitedProducts.Take(5).ToList();
            }
            visitedProducts.Reverse();
            string visitedProductsString = string.Join(",", visitedProducts);
            HttpContext.Session.SetString(CDictionary.SK_PRODUCT_VISITED_ID, visitedProductsString);

            CProductFactory pf = new CProductFactory();
            ProgramViewModel vm = new ProgramViewModel();
            //vm.program.fPhotoList = pf.loadPhoto((int)id);
            vm.program.fPhotoPath = pf.loadPhotoPath((int)id);
            vm.product.ProductName = pf.loadTitle((int)id);
            vm.product.Description = pf.loadDescription((int)id);
            vm.program.fTripDate = pf.loadTrip((int)id);
            vm.product.PlanName = pf.loadPlan((int)id);
            vm.program.fPlanDescription = pf.LoadPlanDescri((int)id);
            vm.product.Address= pf.loadAddress((int)id);
            vm.program.fOutline = pf.loadOutlineDetails((int)id);
            vm.city.City = pf.loadCity((int)id);
            vm.program.fComMem=pf.loadCommentMem((int)id);
            vm.program.fPlanPrice = pf.loadPlanprice((int)id);
            vm.program.fTripDetails = pf.loadTripdetails((int)id);
            vm.program.fTTripPhotoList = pf.loadTripPhoto((int)id);
            vm.program.fTripPrice = pf.loadPlanpriceStart((int)id);
            vm.program.fComMemGender= pf.memgender((int)id);
            vm.program.fCommentDate = pf.loadCommentDate((int)id);
            vm.program.fComScore = pf.loadcommentScore((int)id);
            vm.program.fComTitle = pf.loadCommentTitle((int)id);
            vm.program.fComContent = pf.loadCommentContent((int)id);
            vm.program.fStatus = pf.loadStatus((int)id);
            vm.product.ProductId = (int)id;
            vm.program.fProdTags = pf.loadProductTags((int)id);
            vm.program.fCommentPhotoList = pf.loadCommentPhotoPath((int)id);
            vm.program.fTripId = pf.loadTripId((int)id);
            vm.program.floggedInMemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            vm.program.fDiscountPlanPrice = pf.loadDiscountPrice((int)id);
            vm.program.fDiscountPriceDate = pf.loadDiscountPriceDate((int) id);
            vm.program.fMemPic = pf.loadMemPic((int)id);
            vm.program.fMaxnum = pf.loadStock((int)id);
            vm.comment = pf.LoadCommentsForProduct((int)id);

            TraveldateContext db = new TraveldateContext();
                int memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
                var favorite = db.Favorites
                    .FirstOrDefault(f => f.MemberId == memberID && f.ProductId == id);
            vm.program.fIsFav = favorite!=null;

            return View(vm);
        }



        private List<int> GetVisitedProductsFromSession()
        {
            string visitedProductsString = HttpContext.Session.GetString(CDictionary.SK_PRODUCT_VISITED_ID);

            if (!string.IsNullOrEmpty(visitedProductsString))
            {
                return visitedProductsString.Split(',').Select(int.Parse).ToList();
            }

            return new List<int>();
        }


        public IActionResult Address(int id)
        {
            TraveldateContext db = new TraveldateContext();
            var address = db.ProductLists.Where(p => p.ProductId == id).Select(p => p.Address).ToList();
            return Json(address);
        }

        public IActionResult MaxNum(string selectedDate,int id)
        {
            TraveldateContext db = new TraveldateContext();

            List<DateTime?> tripdate = db.Trips.Where(p=>p.ProductId==id).Select(t => t.Date).ToList();
            List<int?> max = db.Trips.Where(p => p.ProductId == id).Select(t => t.MaxNum).ToList();
            List<int?> min = db.Trips.Where(p => p.ProductId == id).Select(t => t.MinNum).ToList();
            List<decimal?> price = db.Trips.Where(p => p.ProductId == id).Select(t => t.UnitPrice).ToList();
            List<decimal?> discount = db.Trips.Where(p => p.ProductId == id).Select(t => t.Discount).ToList();
            List<int> tripid = db.Trips.Where(p => p.ProductId == id).Select(t => t.TripId).ToList();

            List<int?> quantities = new List<int?>();
            List<int> tripIds = db.Trips
                .Where(p => p.ProductId == id)
                .Select(t => t.TripId)
                .ToList();

            foreach (int tripsId in tripIds)
            {
                int? quantity = db.OrderDetails
                    .Where(o => o.Order.IsCart == false && o.TripId == tripsId)
                    .Sum(o => (int?)o.Quantity); 
                quantities.Add(quantity);
            }

            // 使用 Zip 方法將 maxnum 和 quantities 組合成一個 List<int?>
            List<int?> combinedList = max.Zip(quantities, (m, q) => m - q).ToList();

            int? maxnum = combinedList[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            int? minnum = min[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            decimal? pricenum = price[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            decimal? discountnum = discount[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            int tripId = tripid[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            DateTime? date = tripdate.FirstOrDefault(d => d?.ToString("yyyy-MM-dd") == selectedDate);

            return Json(new
            {
                Maxnum = maxnum,
                Minnum = minnum,
                Price = pricenum,
                Discount = discountnum,
                Date = date?.ToString("yyyy-MM-dd"),
                TripId = tripId
            });
        }


        
        [HttpPost]
        public IActionResult AddToCart(int num, int tripId)
        {
            TraveldateContext db = new TraveldateContext();

            int loggedInMemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            if(loggedInMemberId == 0) {
                return Content("請登入會員");
            }
            var existCart = db.Orders.Any(o => o.MemberId == loggedInMemberId && o.IsCart == true);
            if (existCart)
            {

                if (db.OrderDetails.Any(o => o.TripId == tripId && o.Order.MemberId == loggedInMemberId && o.Order.IsCart == true))
                {
                    OrderDetail od = db.OrderDetails.FirstOrDefault(o => o.TripId == tripId && o.Order.MemberId == loggedInMemberId && o.Order.IsCart == true);
                    od.Quantity += num;
                }
                else
                {
                    int cartOrderID = db.Orders.FirstOrDefault(o => o.MemberId == loggedInMemberId && o.IsCart == true).OrderId;
                    OrderDetail newOrderDetail = new OrderDetail
                    {
                        Quantity = num,
                        TripId = tripId,
                        OrderId = cartOrderID
                    };
                    db.OrderDetails.Add(newOrderDetail);
                }
                db.SaveChanges();
            }
            else
            {
                Order newCartOrder = new Order
                {
                    MemberId = loggedInMemberId,
                    IsCart = true
                };
                db.Orders.Add(newCartOrder);
                db.SaveChanges();
            }

            return RedirectToAction("Product");
        }


        [HttpPost]
        public IActionResult AddDirectToCart(int num, int tripId)
        {
            TraveldateContext db = new TraveldateContext();
            int orderDetailId = 0;

            int loggedInMemberId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            if (loggedInMemberId == 0)
            {
                return Content("請登入會員");
            }


            var existCart = db.Orders.Any(o => o.MemberId == loggedInMemberId && o.IsCart == true);
            if (existCart)
            {

                if (db.OrderDetails.Any(o => o.TripId == tripId && o.Order.MemberId == loggedInMemberId && o.Order.IsCart == true))
                {
                    OrderDetail od = db.OrderDetails.FirstOrDefault(o => o.TripId == tripId && o.Order.MemberId == loggedInMemberId && o.Order.IsCart == true);
                    od.Quantity += num;
                    db.SaveChanges();
                    orderDetailId = od.OrderDetailsId;
                }
                else
                {
                    int cartOrderID = db.Orders.FirstOrDefault(o => o.MemberId == loggedInMemberId && o.IsCart == true).OrderId;
                    OrderDetail newOrderDetail = new OrderDetail
                    {
                        Quantity = num,
                        TripId = tripId,
                        OrderId = cartOrderID
                    };
                    db.OrderDetails.Add(newOrderDetail);
                    db.SaveChanges();
                    //int[] orderDetailId = new int[1];
                    orderDetailId = newOrderDetail.OrderDetailsId;
                    //intorderDetailId = newOrderDetail.OrderDetailsId;


                    ViewBag.orderDetailId = orderDetailId;

                    //return Content(orderDetailId.ToString());
                    //return RedirectToAction("ConfirmOrder", "Cart", new { orderDetailID = orderDetailId });
                    //return RedirectToAction("ShoppingCart", "Cart");
                    //return Json(orderDetailId);
                }
            }
            else
            {
                Order newCartOrder = new Order
                {
                    MemberId = loggedInMemberId,
                    IsCart = true
                };
                db.Orders.Add(newCartOrder);
                db.SaveChanges();

                int newCartOrderID = newCartOrder.OrderId;

                OrderDetail newOrderDetail = new OrderDetail
                {
                    Quantity = num,
                    TripId = tripId,
                    OrderId = newCartOrderID
                };

                db.OrderDetails.Add(newOrderDetail);
                db.SaveChanges();
                //int orderDetailId = newOrderDetail.OrderDetailsId;
                orderDetailId = newOrderDetail.OrderDetailsId;

                ViewBag.orderDetailId = orderDetailId;

                //return Content(orderDetailId.ToString());

                //return RedirectToAction("ConfirmOrder", "Cart", new { id = orderDetailId });

                //return RedirectToAction("ShoppingCart", "Cart");
            }

            return Content(orderDetailId.ToString());


           // return RedirectToAction("ConfirmOrder", "Cart", new { id = orderDetailId });

            //return RedirectToAction("ShoppingCart", "Cart");
        }



        [HttpGet]
        public IActionResult GetTripID(string selectedDate, int productId)
        {
            TraveldateContext db = new TraveldateContext();

            DateTime date = DateTime.ParseExact(selectedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            int tripID = db.Trips
                .Where(t => t.ProductId == productId && t.Date == date)
                .Select(t => t.TripId)
                .FirstOrDefault();

            return Json(tripID);
        }

        [HttpPost]
        public IActionResult FavOrNot(int id)
        {
            TraveldateContext db = new TraveldateContext();
            int memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            if (memberID == 0)
            {
                return Content("請登入會員");
            }

            var existingFavorite = db.Favorites
                .FirstOrDefault(o => o.MemberId == memberID && o.ProductId == id);

            if (existingFavorite == null)
            {
                Favorite favo = new Favorite()
                {
                    MemberId = memberID,
                    ProductId = id
                };
                db.Favorites.Add(favo);
                db.SaveChanges();
                return Json(new { success = true, message = "產品已成功加入最愛。" });
            }
            else
            {
                db.Favorites.Remove(existingFavorite);
                db.SaveChanges();
                return Json(new { success = true, message = "產品已成功移除。" });
            }
        }


    }
}
