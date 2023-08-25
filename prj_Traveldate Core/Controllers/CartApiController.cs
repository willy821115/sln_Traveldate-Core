using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.Controllers
{
    public class CartApiController : SuperController
    {
        int _memberID = 0;
        TraveldateContext _context;
        public CartApiController()
        {
            _context = new TraveldateContext();
        }

        //購物車
        //  加入最愛(修改最愛)
        public IActionResult AddToFavo(int id)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            Favorite favo = new Favorite()
            {
                MemberId = _memberID,
                ProductId = id
            };
            _context.Favorites.Add(favo);
            _context.SaveChanges();
            return RedirectToAction("ShoppingCart", "Cart");
        }
        public IActionResult DeleFromFavo(int id)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var favo = _context.Favorites.Where(o=>o.MemberId==_memberID&&o.ProductId==id).FirstOrDefault();
            if (favo != null)
            {
                _context.Favorites.Remove(favo);
            }
            _context.SaveChanges();
            return RedirectToAction("ShoppingCart", "Cart");
        }
        //  自購物車刪除(刪除OD)
        public IActionResult DeleFromCart(int id)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var od = _context.OrderDetails.Where(o => o.OrderDetailsId==id).FirstOrDefault();
            if (od != null)
            {
                _context.OrderDetails.Remove(od);
            }
            _context.SaveChanges();
            return RedirectToAction("ShoppingCart", "Cart");
        }
        //  刪除所有已選
        [HttpPost]
        public IActionResult DeleAll(int[] orderDetailID)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            for(int i = 0; i < orderDetailID.Length; i++)
            {
                var od = _context.OrderDetails.Where(o => o.OrderDetailsId == orderDetailID[i]).FirstOrDefault();
                if (od != null)
                {
                    _context.OrderDetails.Remove(od);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("ShoppingCart", "Cart");
        }

        //  修改數量
        //  編輯訂購內容(修改OD)


        //確認訂購內容

    }
}
