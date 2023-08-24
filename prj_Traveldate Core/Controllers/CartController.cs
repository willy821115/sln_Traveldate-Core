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
            vm.cartitems = _context.OrderDetails.Where(c => (c.Order.IsCart == true) && (c.Order.MemberId == _memberID)).Select(c=>
            new CCartItem
            {
                orderDetailID = c.OrderDetailsId,
                productID = c.Trip.ProductId,
                tripID = c.TripId,
                planName = c.Trip.Product.ProductName,
                date = $"{c.Trip.Date:d}",
                quantity = c.Quantity,
                photo = c.Trip.Product.ProductPhotoLists.FirstOrDefault().Photo,
                ImagePath = (c.Trip.Product.ProductPhotoLists.FirstOrDefault()!=null) ? c.Trip.Product.ProductPhotoLists.FirstOrDefault().ImagePath : "no_image.png",
                unitPrice = c.Trip.UnitPrice,
                favo = (_context.Favorites.Any(f=>f.MemberId==_memberID&&f.ProductId==c.Trip.ProductId))
        }).ToList();

            //做可以抓推薦欄的Factory in:會員ID out:List<ProductListID>[4/8/12]
            //瀏覽紀錄
            //List加到vm裡顯示
            return View(vm);
        }

        [HttpPost]
        public ActionResult ConfirmOrder(int[] orderDetailID)
        {
            //抓資料: 會員資料+點數 常用旅伴 優惠券
            //抓session: orderDetailID List of checked items
            //合成一個vm 傳給view
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
                    }).First();
                vm.orders.Add(item);
            }

            return View(vm);
        }

        public ActionResult CompleteOrder()
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            ViewData["email"] = _context.Members.Find(_memberID).Email;
            return View();
        }
    }
}
