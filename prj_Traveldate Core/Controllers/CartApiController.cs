using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        // +-按鈕 修改數量
        public IActionResult ItemPlus(int id) //odid
        {
            OrderDetail od = _context.OrderDetails.Find(id);
            if (od != null)
            {
                od.Quantity += 1;
                _context.SaveChanges();
                return Content(od.Quantity.ToString());
            }
            return NoContent();
        }

        public IActionResult ItemMinus(int id) //odid
        {
            OrderDetail od = _context.OrderDetails.Find(id);
            if (od != null)
            {
                if (od.Quantity > 1)
                {
                    od.Quantity -= 1;
                    _context.SaveChanges();
                    return Content(od.Quantity.ToString());
                }
            }
            return NoContent();
        }

        //TODO  編輯訂購內容(修改OD)
        public IActionResult LoadTrips(int id) //tripid
        {

            var pid = _context.Trips.Find(id).ProductId;
            var trips = _context.Trips.Where(t => t.ProductId == pid && t.Date > DateTime.Now).OrderBy(t => t.Date).Select(t => new
            {
                tripId = t.TripId,
                date = t.Date,
                unitPrice = t.UnitPrice,
                discount = t.Discount?? 0,
                maxNum = t.MaxNum,
                stock = t.MaxNum - _context.Trips.Where(s => s.TripId == t.TripId).Select(s => s.OrderDetails.Where(o => o.Order.IsCart == false).Sum(o => o.Quantity)).FirstOrDefault()
        }).ToList();

            return Json(trips);
        }

        //修改資料庫
        public IActionResult EditCart(int odid, int num, int tripid)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var exixt = _context.OrderDetails.Where(o=>o.Order.MemberId==_memberID && o.Order.IsCart==true && o.TripId==tripid && o.OrderDetailsId!= odid).FirstOrDefault();
            if (exixt != null)
            {
                return Content("exist");
            }
            else
            {
                OrderDetail thisod = _context.OrderDetails.Find(odid);
                thisod.TripId = tripid;
                thisod.Quantity = num;
                _context.SaveChanges();
                return Content("OK");
            }

        }


        //TODO  抓推薦
        public List<int> recommendPlid(int mID)
        {
            List<int> idList = new List<int>();

            //已訂購過或在購物車的商品ID
            List<int> buyed = _context.OrderDetails.Where(o => o.Order.MemberId == mID).Select(o => o.Trip.ProductId).Distinct().ToList();

            //buyed的商品的tags
            List<int> tags = new List<int>();
            foreach (var tag in _context.ProductTagLists)
            {
                if (buyed.Contains((int)tag.ProductId))
                {
                    tags.Add((int)tag.ProductTagDetailsId);
                }
            }

            Random rnd = new Random();
            while (idList.Count < 5)
            {
                if (tags.Count == 0)
                    break;
                //隨機選一個tag
                int n = rnd.Next(tags.Count);
                var ps = _context.ProductTagLists.Where(o => o.ProductTagDetailsId == tags[n]).Select(o => o.ProductId).Distinct().ToList();
                if (ps.Any())
                {
                    int m = rnd.Next(ps.Count);
                    if (!idList.Contains((int)ps[m]))
                    {
                        if(_context.Trips.Any(t=>t.ProductId == (int)ps[m]))
                        {
                            idList.Add((int)ps[m]);
                            ps.RemoveAt(m);
                        }
                    }
                }
            }

            if (idList.Count < 5)
            {
                var plist = _context.OrderDetails.GroupBy(o => o.Trip.ProductId).Select(g => new { prodID = g.Key, ocount = g.Count() }).OrderByDescending(x => x.ocount).Take(5).ToList();
                for (int i = 0; i < plist.Count(); i++)
                {
                    idList.Add(plist[i].prodID);
                }
            }
            return idList;
        }


        //透過ID取得同行者資料
        public IActionResult GetCompanionByID(int id)
        {
            var companion = _context.Companions.Where(c=>c.CompanionId==id).Select(c=>c).FirstOrDefault();
            return Json(companion);
        }

        //取得購物車內商品數量
        [HttpGet]
        public IActionResult GetCartCount()
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            var count = _context.OrderDetails.Where(o => o.Order.IsCart == true && o.Order.MemberId == _memberID).Count();
            return Content(count.ToString());
        }
    }
}
