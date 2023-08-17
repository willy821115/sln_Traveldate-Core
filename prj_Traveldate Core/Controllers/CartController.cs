using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class CartController : Controller
    {
        int _memberID = 1;
        TraveldateContext _context;
        public CartController()
        {
            _context = new TraveldateContext();
        }
        public ActionResult ShoppingCart()
        {
            CShoppingCartViewModel vm = new CShoppingCartViewModel();
            vm.cartitems = new List<CCartItem>();
            vm.cartitems = _context.OrderDetails.Where(c => (c.Order.IsCart == true) && (c.Order.MemberId == _memberID)).Select(c=>
            new CCartItem
            {
                orderDetailID = c.OrderDetailsId,
                planName = c.Trip.Product.ProductName,
                date = $"{c.Trip.Date:d}",
                quantity = c.Quantity,
                photo = c.Trip.Product.ProductPhotoLists.FirstOrDefault().Photo,
                unitPrice = c.Trip.UnitPrice
        }).ToList();

            //做可以抓推薦欄的Factory in:會員ID out:List<ProductListID>[4/8/12]
            //List加到vm裡顯示
            return View(vm);
        }
        [HttpPost]
        public ActionResult ShoppingCart(int id)
        {
            //取得勾選的items 將odid的List 傳入下一頁 (用TempData?)
            return RedirectToAction("ConfirmOrder");
        }

        public ActionResult ConfirmOrder()
        {
            //抓資料: 會員資料+點數 常用旅伴 優惠券
            //抓session: orderDetailID List of checked items
            //合成一個vm 傳給view

            CConfirmOrderViewModel vm = new CConfirmOrderViewModel();
            vm.member = _context.Members.Find(_memberID);
            vm.companions = _context.Companions.Where(c => c.MemberId == _memberID).ToList();
            vm.coupons = _context.Coupons.Where(c => c.MemberId == _memberID).Select(c => c.CouponList).ToList();
            //vm.checkedItems

            return View(vm);
        }
        [HttpPost]
        public ActionResult ConfirmOrder(int a)
        {
            //接API??
            //寄email??
            return RedirectToAction("CompleteOrder");
        }
        public ActionResult CompleteOrder()
        {
            ViewData["email"] = _context.Members.Find(_memberID).Email;
            return View();
        }
    }
}
