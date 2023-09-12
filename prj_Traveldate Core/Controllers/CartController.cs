using ECPAYTEST.Domain;
using ECPAYTEST.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Caching.Memory;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using RazorEngine;
using RazorEngine.Templating;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace prj_Traveldate_Core.Controllers
{
    public class CartController : SuperController
    {
        int _memberID = 0;
        TraveldateContext _context;
        private IWebHostEnvironment _enviro;
        private readonly IConfiguration _configuration;
        private readonly LinePayService _linePayService;
        private readonly IMemoryCache _cache;

        public CartController(IWebHostEnvironment p, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _enviro = p;
            _configuration = configuration;
            _cache = memoryCache;
            _context = new TraveldateContext();
            _linePayService = new LinePayService(_cache);

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
            vm.cartitems = _context.OrderDetails.Where(c => (c.Order.IsCart == true) && (c.Order.MemberId == _memberID) && (c.Trip.Date>DateTime.Now)).Select(c =>
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
            CartApiController api = new CartApiController();
            var ps = api.recommendPlid(_memberID); //推薦的商品ID們

            List<CHomeViewModel> recommendList = new List<CHomeViewModel>();
            foreach (var p in ps)
            {
                CHomeViewModel item = new CHomeViewModel();
                item.productId = p;
                item.productName = _context.ProductLists.Find(p).ProductName;
                item.unitPrice = (decimal)(_context.Trips.Where(t => t.ProductId == p).OrderBy(t => t.UnitPrice).Select(t => t.UnitPrice).FirstOrDefault() ?? 0);

                if(_context.CommentLists.Where(c => c.ProductId == p).Any())
                {
                    item.commentScore = _context.CommentLists.Where(c => c.ProductId == p).Select(c => c.CommentScore).Average();
                    item.commentScoreString = string.Format("{0:F1}", item.commentScore) + "/5";
                }
                else
                {
                    item.commentScoreString = "尚無評價";
                }
                var imagePath = _context.ProductPhotoLists.Where(c => c.ProductId == p).Select(c => c.ImagePath).FirstOrDefault();

                item.ImagePath = imagePath;
                
                recommendList.Add(item);
            }
            vm.recommends = recommendList;


            var visitedProductIds = HttpContext.Session.GetString(CDictionary.SK_PRODUCT_VISITED_ID)?.Split(',');

            List<CHomeViewModel> browsingList = new List<CHomeViewModel>();

            if (visitedProductIds != null)
            {
                foreach (var productId in visitedProductIds)
                {
                    if (int.TryParse(productId, out int p))
                    {
                        CHomeViewModel item = new CHomeViewModel();
                        item.productId = p;
                        item.productName = _context.ProductLists.Find(p)?.ProductName;
                        item.unitPrice = (decimal)(_context.Trips.Where(t => t.ProductId == p).OrderBy(t => t.UnitPrice).Select(t => t.UnitPrice).FirstOrDefault() ?? 0);

                        if (_context.CommentLists.Where(c => c.ProductId == p).Any())
                        {
                            item.commentScore = _context.CommentLists.Where(c => c.ProductId == p).Select(c => c.CommentScore).Average();
                            item.commentScoreString = string.Format("{0:F1}", item.commentScore) + "/5";
                        }
                        else
                        {
                            item.commentScoreString = "尚無評價";
                        }

                        var imagePath = _context.ProductPhotoLists.Where(c => c.ProductId == p).Select(c => c.ImagePath).FirstOrDefault();
                        item.ImagePath = imagePath;

                        browsingList.Add(item);
                    }
                }
                vm.browsing = browsingList;
            }
            else vm.browsing = null;

            return View(vm);
        }

        [HttpPost]
        [Route("Cart/Checkout")]
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

        public ActionResult ConfirmOrder(int id)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));

            CConfirmOrderViewModel vm = new CConfirmOrderViewModel();
            vm.member = _context.Members.Find(_memberID);
            vm.companions = _context.Companions.Where(c => c.MemberId == _memberID).ToList();

            vm.coupons = _context.Coupons.Where(c => c.MemberId == _memberID && c.CouponList.DueDate > DateTime.Now).Select(c => c.CouponList).ToList();

            vm.orders = new List<CCartItem>();
                CCartItem item = new CCartItem();
                item = _context.OrderDetails.Where(o => o.OrderDetailsId == id).Select(c =>
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
            

            return View(vm);
        }

        //揪團結帳
        [Route("Cart/ForumCheckout")]
        public IActionResult ConfirmOrder(int ForumListID,int from, int type=0)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            List<int> orderDetailIdList = new List<int>();
            List<int?> tripId = new List<int?>();

            if (type == 0)
            {
                var list = _context.ScheduleLists.Where(l => l.ForumListId == ForumListID).Select(l => l.TripId).Distinct().ToList();
                if (list != null)
                {
                    tripId = list;
                }
                else return Content("");

                var existCart = _context.Orders.Any(o => o.MemberId == _memberID && o.IsCart == true);
                if (existCart)
                {
                    int cartOrderID = _context.Orders.FirstOrDefault(o => o.MemberId == _memberID && o.IsCart == true).OrderId;

                    foreach (var i in tripId)
                    {
                        OrderDetail newOrderDetail = new OrderDetail
                        {
                            Quantity = 1,
                            TripId = i,
                            OrderId = cartOrderID
                        };
                        _context.OrderDetails.Add(newOrderDetail);
                        _context.SaveChanges();
                        orderDetailIdList.Add(newOrderDetail.OrderDetailsId);
                    }
                }
                else
                {
                    Order newCartOrder = new Order
                    {
                        MemberId = _memberID,
                        IsCart = true
                    };
                    _context.Orders.Add(newCartOrder);
                    _context.SaveChanges();

                    int newCartOrderID = newCartOrder.OrderId;

                    foreach (var i in tripId)
                    {
                        OrderDetail newOrderDetail = new OrderDetail
                        {
                            Quantity = 1,
                            TripId = i,
                            OrderId = newCartOrderID
                        };
                        _context.OrderDetails.Add(newOrderDetail);
                        _context.SaveChanges();
                        orderDetailIdList.Add(newOrderDetail.OrderDetailsId);
                    }
                }

                CConfirmOrderViewModel vm = new CConfirmOrderViewModel();
                vm.member = _context.Members.Find(_memberID);
                vm.companions = _context.Companions.Where(c => c.MemberId == _memberID).ToList();

                vm.coupons = _context.Coupons.Where(c => c.MemberId == _memberID && c.CouponList.DueDate > DateTime.Now).Select(c => c.CouponList).ToList();

                vm.orders = new List<CCartItem>();
                for (int i = 0; i < orderDetailIdList.Count; i++)
                {
                    CCartItem item = new CCartItem();
                    item = _context.OrderDetails.Where(o => o.OrderDetailsId == orderDetailIdList[i]).Select(c =>
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
                if (from == 0)
                {
                    //把forumlistId帶到結帳成功那邊再把文章的isPublish改成true
                    HttpContext.Session.SetInt32(CDictionary.SK_FORUMLISTID_FOR_PAY, ForumListID);
                }
                if(from == 1)
                {
                    //for跟團
                    HttpContext.Session.SetInt32(CDictionary.SK_FORUMLISTID_FOR_PAY_JOIN, ForumListID);
                }
                if (from == 2) 
                {
                    //forDemo
                    //將文章id儲存在 ViewBag.demoPay 到結帳畫面中取出並返回剛才發布的文章
                    ViewBag.demoPay =  ForumListID;
                }


                return View(vm);
            }
            return Content("");
        }

        

        [HttpPost]
        public ActionResult Payment(CCreateOrderViewModel vm)
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            int oid;

            //產生付款ID填入Payment欄位
            var orderIdForPay = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);

            DateTime dt = DateTime.Now;

            for (int i = 0; i < vm.ods.Count(); i++)
            {
                int tripID = vm.ods[i].TripId ?? 0;
                //確認商品庫存量
                var q = _context.Trips.Where(s => s.TripId == tripID).Select(s => new { orders = s.OrderDetails.Where(o => o.Order.IsCart == false).Sum(o => o.Quantity), max = s.MaxNum }).FirstOrDefault();

                if (q.max - q.orders < vm.ods[i].Quantity)
                {
                    string prodName = _context.Trips.Where(t => t.TripId == vm.ods[i].TripId).Select(t => t.Product.ProductName).FirstOrDefault();
                    string e = $"「{prodName}」數量不足，請重新選購。";
                    return RedirectToAction("OrderError", new { e });
                }
            }

            using (var tran = _context.Database.BeginTransaction())
            {
                EcpayOrder ecpayOrder = new EcpayOrder()
                {
                    MerchantTradeNo = orderIdForPay,
                    MemberId = "2000132",
                    RtnCode = 0, //未付款
                    RtnMsg = "訂購成功尚未付款",
                    TradeNo = "2000132",
                    TradeAmt = Decimal.ToInt32(vm.checkoutAmount),
                    PaymentType = "aio",
                    PaymentTypeChargeFee = "0",
                    TradeDate = dt.ToString("yyyy/MM/dd HH:mm:ss"),
                    SimulatePaid = 0,
                
                 };
                

                Order newOrder = new Order()
                {
                    MemberId = _memberID,
                    Datetime = DateTime.Now,
                    CouponListId = vm.newOrder.CouponListId, //
                    Discount = vm.newOrder.Discount, //
                    PaymentId = orderIdForPay, //
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

                _context.EcpayOrders.Add(ecpayOrder);
                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                oid = newOrder.OrderId;

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

                    //刪除購物車裡的項目
                    OrderDetail? oldOd = _context.OrderDetails.Where(o => o.OrderDetailsId == vm.ods[i].OrderDetailsId).FirstOrDefault();
                    if (oldOd != null)
                    {
                        _context.OrderDetails.Remove(oldOd);
                    }

                    _context.OrderDetails.Add(newOd);
                    _context.SaveChanges();

                    if (vm.companions!=null && vm.companions[i] != null)
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

            //將此筆訂單編號存入Session
            HttpContext.Session.SetString(CDictionary.SK_PAID_ORDER_ID, oid.ToString());

            //準備付款所需資料
            //網址
            string website = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
            string itemName = "";

            for (int i = 0; i < vm.ods.Count; i++)
            {
                itemName += _context.Trips.Where(t => t.TripId == vm.ods[i].TripId).Select(t => t.Product.ProductName).First() + "#";
            }

            var order = new Dictionary<string, string>
            {
                //綠界需要的參數
                { "MerchantTradeNo",  orderIdForPay},
                { "MerchantTradeDate",  dt.ToString("yyyy/MM/dd HH:mm:ss")},
                { "TotalAmount",  Math.Floor(vm.checkoutAmount).ToString()},
                { "TradeDesc",  "無"},
                { "ItemName",  itemName},
                { "CustomField1",  ""},
                { "CustomField2",  ""},
                { "CustomField3",  ""},
                { "CustomField4",  ""},
                { "ReturnURL",  $"{website}/api/Ecpay/AddPayInfo"},
                { "OrderResultURL", $"{website}/Ecpay/PayInfo/{orderIdForPay}"},
                { "PaymentInfoURL",  $"{website}/api/Ecpay/AddAccountInfo"},
                { "ClientRedirectURL",  $"{website}/Ecpay/AccountInfo/{orderIdForPay}"},
                { "MerchantID",  "2000132"},
                { "IgnorePayment",  ""},
                { "PaymentType",  "aio"},
                { "ChoosePayment",  "ALL"},
                { "EncryptType",  "1"},
            };
            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);
            return View(order);
        }

        public IActionResult CompleteOrder()
        {
            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            ViewData["email"] = _context.Members.Find(_memberID).Email;
            //寄確認信
            SendOrderMail();

            //如果是從揪團發文過來的走這裡
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_FORUMLISTID_FOR_PAY))
            {
                int? ForumListID = HttpContext.Session.GetInt32(CDictionary.SK_FORUMLISTID_FOR_PAY);
                var createArticle = _context.ForumLists.Find(ForumListID);
                createArticle.IsPublish = true;
                createArticle.ReleaseDatetime = DateTime.Now;
                _context.SaveChanges();
                return RedirectToAction("ArticleView", "Forum", new {id= ForumListID, returnType = 0});
            }
            //如果是從跟團過來的走這裡
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_FORUMLISTID_FOR_PAY_JOIN))
            {
                int? ForumListID = HttpContext.Session.GetInt32(CDictionary.SK_FORUMLISTID_FOR_PAY_JOIN);

                return RedirectToAction("ArticleView", "Forum", new { id = ForumListID, returnType = 1 });
            }

            return View();
        }


        public IActionResult OrderError(string e)
        {

            ViewBag.message = e;
            return View();
        }

        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);
            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";
            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";
            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);
            return checkValue.ToUpper();
        }

        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = System.Text.Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }


        public void SendOrderMail()
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_PAID_ORDER_ID))
            {
                return;
            }

            _memberID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
            List<string> UserEmail = new List<string>
            {
                _context.Members.Find(_memberID).Email
            };
            string templatePath = "Views/Emails/OrderEmailTemplate.cshtml";
            int oid = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_PAID_ORDER_ID));
            HttpContext.Session.Remove(CDictionary.SK_PAID_ORDER_ID);

            LoginApiController api = new LoginApiController(_configuration, HttpContext);


            //開始用oid抓資料/////////////////////////////////////

            COrderEmailViewModel vm = new COrderEmailViewModel();
            var orderdata = _context.Orders.Where(o => o.OrderId == oid)
                .Select(o => new {
                    name = o.Member.FirstName,
                    ods = _context.OrderDetails.Where(od => od.OrderId == o.OrderId).ToList(),
                    point = (o.Discount == null) ? 0 : o.Discount,
                    coupon = (o.CouponListId == null) ? 0 : o.CouponListId,
                }).First();
            
            
            vm.userName = orderdata.name;
            vm.orders = new List<COrderMail>();
            //建立連結資源
            List<LinkedResource> linkedrs = new List<LinkedResource>();

            foreach(var i in orderdata.ods)
            {
                var od = _context.OrderDetails.Where(o => o.OrderDetailsId == i.OrderDetailsId).Select(o => new COrderMail
                {
                    planName = o.Trip.Product.PlanName,
                    date = $"{o.Trip.Date:d}",
                    unitPrice = Decimal.ToInt32((decimal)o.SellingPrice),
                    quantity = (int)o.Quantity,
                    imageFileName = _context.ProductPhotoLists.Where(p => p.ProductId == o.Trip.ProductId).Select(p=>p.ImagePath).First()
                }).FirstOrDefault();

                string imagePath = _enviro.WebRootPath + "/images/" + od.imageFileName;

                vm.total += od.unitPrice * od.quantity;
                
                var res = new LinkedResource(imagePath);
                res.ContentId = Guid.NewGuid().ToString();
                linkedrs.Add(res);
                //使用<img src="/img/loading.svg" data-src="cid:..."方式引用內嵌圖片
                od.imagePath = $@"cid:{res.ContentId}";
                vm.orders.Add(od);

            }

            double temp = vm.total;
            double discount = (orderdata.coupon==0)? 1 : Decimal.ToDouble((decimal)_context.CouponLists.Where(c=>c.CouponListId==orderdata.coupon).Select(c=>c.Discount).FirstOrDefault());
            vm.buttonLink = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.ToString() + "/Member/orderList";
            vm.coupon = (int)Math.Ceiling(temp * (1 - discount));
            vm.point = (int)orderdata.point;
            vm.total = vm.total - vm.coupon - vm.point;


            /////////////////////////////////////////////////
            string mailContent = Engine.Razor.RunCompile(System.IO.File.ReadAllText(templatePath), "ordermail", typeof(COrderEmailViewModel), vm);
            
            //建立AlternativeView
            var altView = AlternateView.CreateAlternateViewFromString(
                mailContent, null, MediaTypeNames.Text.Html);
            //將圖檔資源加入AlternativeView
            foreach(var r in linkedrs)
            {
                altView.LinkedResources.Add(r);
            }

            string mailSubject = "您的 Traveldate 訂單明細";
            api.SimplySendMail(mailSubject, UserEmail, altView);

        }

        public async Task<IActionResult> LinePay()
        {
            int oid = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_PAID_ORDER_ID));
            string website = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;

            Order o = _context.Orders.Find(oid);

            PaymentRequestDto dto = new PaymentRequestDto();
            dto.Amount = 0;
            dto.Currency = "TWD";
            dto.OrderId = DateTime.Now.ToString(); //使用 Timestamp 當作 orderId
            dto.Packages = new List<PackageDto>();
            dto.RedirectUrls = new RedirectUrlsDto();

            var packages = new PackageDto()
            {
                Id = oid.ToString(),
                Amount = 0,
                Name = "購買的旅程",
                Products = new List<LinePayProductDto>(),
            };

            var ods = _context.OrderDetails.Where(o => o.OrderId == oid).ToList();

            foreach(var od in ods)
            {
                LinePayProductDto pds = new LinePayProductDto();
                pds.Name = _context.OrderDetails.Where(o=>o.OrderDetailsId==od.OrderDetailsId).Select(o=>o.Trip.Product.ProductName).First();
                pds.Quantity = (int)od.Quantity;
                pds.OriginalPrice = (int)od.SellingPrice;
                pds.Price = (int)od.SellingPrice;
                int pid = _context.OrderDetails.Where(o => o.OrderDetailsId == od.OrderDetailsId).Select(o => o.Trip.ProductId).First();
                pds.ImageUrl = _enviro.WebRootPath + "/images/" + _context.ProductPhotoLists.Where(p => p.ProductId == pid).Select(p => p.ImagePath).First();

                packages.Products.Add(pds);
                packages.Amount += pds.Price * pds.Quantity;
                dto.Amount += pds.Price * pds.Quantity;
            }
            dto.Packages.Add(packages);

            dto.RedirectUrls.ConfirmUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/Cart/ConfirmLPPayment";

            _cache.Set<Int32>("amount", dto.Amount);

            var r = await _linePayService.SendPaymentRequest(dto);
            return Redirect(r.Info.PaymentUrl.Web);

        }

        public async Task<IActionResult> CreatePayment(PaymentRequestDto dto)
        {
            var r = await _linePayService.SendPaymentRequest(dto);
            return Redirect(r.Info.PaymentUrl.Web);
        }

        public async Task<IActionResult> ConfirmLPPayment([FromQuery] string transactionId, [FromQuery] string orderId)
        {
            var result =  await _linePayService.ConfirmPayment(transactionId, orderId);
            return RedirectToAction("CompleteOrder");
        }

    }
}

