using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class CartController : SuperController
    {
        int _memberID = 0;
        TraveldateContext _context;
        public CartController()
        {
            _context = new TraveldateContext();

            ////////////////////////////////
            ////先填入所有sellingprice

            //var ods = _context.OrderDetails.Where(o=>o.Order.IsCart!=true).ToList();
            //foreach (var i in ods)
            //{
            //    if (i.SellingPrice == null)
            //    {
            //        i.SellingPrice = _context.Trips.Where(t => t.TripId == i.TripId).Select(t => t.UnitPrice).First();
            //    }
            //}
            //_context.SaveChanges();

            ////////////////////////////////

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

            vm.coupons = _context.Coupons.Where(c => c.MemberId == _memberID && c.CouponList.DueDate>DateTime.Now).Select(c => c.CouponList).ToList();

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
                        ProductTypeID = c.Trip.Product.ProductTypeId,
                    }).First();
                vm.orders.Add(item);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult CompleteOrder(CCreateOrderViewModel vm)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));

            using (var tran = _context.Database.BeginTransaction())
            {
                Order newOrder = new Order()
                {
                    MemberId = _memberID,
                    Datetime = DateTime.Now,
                    CouponListId = vm.newOrder.CouponListId, //
                    Discount = vm.newOrder.Discount, //
                    PaymentId = vm.newOrder.PaymentId, //
                    IsCart = false
                };

                //刪優惠券
                Coupon? c = _context.Coupons.Where(c=>c.MemberId==_memberID && c.CouponListId == vm.newOrder.CouponListId).FirstOrDefault();
                if (c != null)
                {
                    _context.Coupons.Remove(c); 
                }

                //扣點數
                Member mem = _context.Members.Where(m=>m.MemberId==_memberID).First();
                mem.Discount -= vm.newOrder.Discount;

                //加點數
                mem.Discount += Decimal.ToInt32(Math.Ceiling(vm.checkoutAmount / 100));                


                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                for (int i = 0; i < vm.ods.Count(); i++)
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

                    //減掉商品數量
                    Trip? trip = _context.Trips.Where(t => t.TripId == vm.ods[i].TripId).FirstOrDefault();
                    if (trip != null && trip.MaxNum >= vm.ods[i].Quantity)
                    {
                        trip.MaxNum -= vm.ods[i].Quantity;
                    }

                    //刪除購物車裡的項目
                    OrderDetail? oldOd = _context.OrderDetails.Where(o => o.OrderDetailsId == vm.ods[i].OrderDetailsId).FirstOrDefault();
                    if (oldOd != null)
                    {
                        _context.OrderDetails.Remove(oldOd);
                    }

                    _context.OrderDetails.Add(newOd);
                    _context.SaveChanges();

                    if (vm.companions[i] != null)
                    {
                        for (int j = 0; j < vm.companions[i].Count(); j++)
                        {
                            Companion cpn = vm.companions[i][j];
                            if (cpn != null && cpn.LastName != null && cpn.FirstName != null && cpn.CompanionId == 0)
                            {
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
                            else if (cpn != null && cpn.CompanionId != 0)
                            {
                                CompanionList cpnList = new CompanionList()
                                {
                                    OrderDetailsId = newOd.OrderDetailsId,
                                    CompanionId = cpn.CompanionId
                                };
                                _context.CompanionLists.Add(cpnList);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
                tran.Commit();
            }

            //計算累積消費金額 判斷階級 更新階級
            Member m = _context.Members.Where(m=>m.MemberId.Equals(_memberID)).First();
            var orderHis = _context.Orders.Where(o => o.MemberId == _memberID && o.IsCart == false).ToList();
            decimal[] consumption = new decimal[orderHis.Count];
            decimal sum = 0;
            for (int i = 0; i < orderHis.Count; i++)
            {
                decimal orderTotal = (decimal)_context.OrderDetails.Where(od => od.OrderId == orderHis[i].OrderId).Sum(od => od.SellingPrice);
                int disc = (orderHis[i].Discount != null) ? (int)orderHis[i].Discount : 0;
                decimal? cpamount = _context.CouponLists.Where(cl => cl.CouponListId == orderHis[i].CouponListId).Select(d => d.Discount).FirstOrDefault();
                if (cpamount != null && cpamount < 1 && cpamount > 0)
                {
                    consumption[i] = orderTotal * (decimal)cpamount - disc;
                }
                else if (cpamount != null && cpamount >= 1)
                {
                    consumption[i] = orderTotal - (decimal)cpamount - disc;
                }
                else if (cpamount == null)
                {
                    consumption[i] = orderTotal - disc;
                }
                sum += consumption[i];
            }
            var levels = _context.LevelLists.OrderByDescending(l=>l.Standard).ToList();
            foreach (var level in levels)
            {
                if (sum >= level.Standard)
                {
                    m.LevelId = level.LevelId;
                    _context.SaveChanges();
                    break;
                }
            }

            ViewData["email"] = _context.Members.Find(_memberID).Email;
            
            //TODO 寄確認信
            
            return View();
        }

        public IActionResult OrderError(string e)
        {

            ViewBag.message = e;
            return View();
        }
    }
}
