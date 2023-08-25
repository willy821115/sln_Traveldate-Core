using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

//抓推薦欄 抓瀏覽紀錄
//購物車刪除 愛心 修改 API
//確認訂單
//結帳後加入購物車 確認內容 扣點數 加點數 累積消費

namespace prj_Traveldate_Core.Controllers
{
    public class CartController : SuperController
    {
        int _memberID = 0;
        TraveldateContext _context;
        public CartController()
        {
            _context = new TraveldateContext();
        }
        public ActionResult ShoppingCart()
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));

            CShoppingCartViewModel vm = new CShoppingCartViewModel();
            vm.cartitems = new List<CCartItem>();
            vm.cartitems = _context.OrderDetails.Where(c => (c.Order.IsCart == true) && (c.Order.MemberId == _memberID)).Select(c =>
            new CCartItem
            {
                orderDetailID = c.OrderDetailsId,
                productID = c.Trip.ProductId,
                tripID = c.TripId,
                planName = c.Trip.Product.ProductName,
                date = $"{c.Trip.Date:d}",
                quantity = c.Quantity,
                photo = c.Trip.Product.ProductPhotoLists.FirstOrDefault().Photo,
                ImagePath = (c.Trip.Product.ProductPhotoLists.FirstOrDefault() != null) ? c.Trip.Product.ProductPhotoLists.FirstOrDefault().ImagePath : "no_image.png",
                unitPrice = c.Trip.UnitPrice,
                discount = (c.Trip.Discount != null)? c.Trip.Discount : 0,
                favo = (_context.Favorites.Any(f => f.MemberId == _memberID && f.ProductId == c.Trip.ProductId))
            }).ToList();

            //做可以抓推薦欄的Factory in:會員ID out:List<ProductListID>[4/8/12]
            //  個人化推薦(抓OD 搜尋商品 回傳)
            //  瀏覽紀錄(抓Session回傳)
            //  List加到vm裡顯示
            return View(vm);
        }

        [HttpPost]
        public ActionResult ConfirmOrder(int[] orderDetailID)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));

            CConfirmOrderViewModel vm = new CConfirmOrderViewModel();
            vm.member = _context.Members.Find(_memberID);
            vm.companions = _context.Companions.Where(c => c.MemberId == _memberID).ToList();
            vm.coupons = _context.Coupons.Where(c => c.MemberId == _memberID).Select(c => c.CouponList).ToList();

            vm.orders = new List<CCartItem>();
            for(int i = 0; i < orderDetailID.Length; i++)
            {
                CCartItem item = new CCartItem();
                item = _context.OrderDetails.Where(o=>o.OrderDetailsId == orderDetailID[i]).Select(c =>
                    new CCartItem
                    {
                        orderDetailID = c.OrderDetailsId,
                        productID = c.Trip.ProductId,
                        tripID = c.TripId,
                        planName = c.Trip.Product.ProductName,
                        date = $"{c.Trip.Date:d}",
                        quantity = c.Quantity,
                        photo = c.Trip.Product.ProductPhotoLists.FirstOrDefault().Photo,
                        ImagePath = (c.Trip.Product.ProductPhotoLists.FirstOrDefault() != null) ? c.Trip.Product.ProductPhotoLists.FirstOrDefault().ImagePath : "no_image.png",
                        unitPrice = c.Trip.UnitPrice,
                        discount = (c.Trip.Discount != null) ? c.Trip.Discount : 0,
                    }).First();
                vm.orders.Add(item);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult CompleteOrder(CCreateOrderViewModel vm)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));

            //減掉每個商品數量
            //刪優惠券
            //扣點數
            //加點數
            //計算累積消費金額 判斷階級 更新階級

            Order newOrder = new Order()
            {
                MemberId = _memberID,
                Datetime = DateTime.Now,
                CouponListId = vm.newOrder.CouponListId, //
                Discount = vm.newOrder.Discount, //
                PaymentId = vm.newOrder.PaymentId, //
                IsCart = false
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            for(int i = 0; i < vm.ods.Count(); i++)
            {
                OrderDetail newOd = new OrderDetail()
                {
                    OrderId = newOrder.OrderId,
                    Quantity = vm.ods[i].Quantity, //
                    StatusId = 1,
                    TripId = vm.ods[i].TripId, //
                    SellingPrice = vm.ods[i].SellingPrice, //
                    Note = vm.ods[i].Note
                };

                _context.OrderDetails.Add(newOd);
                _context.SaveChanges();

                if (vm.companions[i] != null)
                {
                    for(int j = 0; j < vm.companions[i].Count(); j++)
                    {
                        Companion cpn = vm.companions[i][j];
                        cpn.MemberId = _memberID;
                        _context.Companions.Add(cpn);
                        _context.SaveChanges();

                        CompanionList cpnList = new CompanionList()
                        {
                            OrderDetailsId = newOd.OrderDetailsId,
                            CompanionId = cpn.CompanionId
                        };
                        _context.CompanionLists.Add(cpnList);
                        _context.SaveChanges();
                    }
                }
                if (vm.companionLists[i] != null)
                {
                    for(int j = 0; j< vm.companionLists[i].Count(); j++)
                    {
                        CompanionList cpnList = vm.companionLists[i][j];
                        cpnList.OrderDetailsId = newOd.OrderDetailsId;
                        _context.CompanionLists.Add(cpnList);
                        _context.SaveChanges();
                    }
                }
            }

            ViewData["email"] = _context.Members.Find(_memberID).Email;
            return View();
        }
    }
}
