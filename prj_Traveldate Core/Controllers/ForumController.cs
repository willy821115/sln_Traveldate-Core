using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Plugins;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prj_Traveldate_Core.Controllers
{
    public class ForumController : Controller
    {
        CForumListViewModel vm = new CForumListViewModel();
        private TraveldateContext _context;
        public ForumController(TraveldateContext context)
        {
            _context = context;

        }
        //文章照片
        List<CForumList_prodPhoto> forum_prodPhoto()
        {
            var tripId = _context.ScheduleLists.Where(s => s.TripId == s.Trip.TripId).Select(s => s.Trip.Product.ProductId).ToList();
            List<CForumList_prodPhoto> prod_photo = _context.ProductPhotoLists.Include(p=>p.Product).Where(p => tripId.Contains((int)p.ProductId)).Select(p => new CForumList_prodPhoto
            {
                prodId = (int)p.ProductId,
                prodPhotoPath = p.ImagePath
            }).ToList();
            return prod_photo;
        }
        //要顯示文章的主要資料
        public List<ScheduleList1> ScheduleForum()
        {
            try
            {
List<ScheduleList1> data = _context.ScheduleLists
                .Include(s => s.ForumList)
                .Include(s => s.Trip)
                .Include(s=>s.ForumList.ReplyLists)
                .Include(s => s.ForumList.Member)
                .Include(s => s.Trip.Product)
                .Include(s => s.Trip.Product.City)
                .Include(s => s.ForumList.Member.Level)
                .Include(s => s.Trip.Product.ProductTagLists)
                .Where(s => s.ForumList.IsPublish == true)
                .GroupBy(g => g.ForumListId)
                .Select(g => new ScheduleList1
                {
                    forumListId = g.Key,
                    trips = g.Select(s => s.Trip).ToList(),
                    ForumList = g.First().ForumList,// 第一個 Trip
                    productTags = g.SelectMany(t=>t.Trip.Product.ProductTagLists).ToList()
                })
                .ToList();

           // var dtats = _context.ProductTagLists.Include(t => t.ProductTagDetails).ToList();
            return data;
            }catch(Exception ex)
            {
                throw new Exception("發生 IOException 異常: " + ex.Message);
            }
            
        }
        //要顯示文章的其他資料
        private void forumInfos()
        {
            try
            {
              vm.replyList = _context.ReplyLists.ToList();
            vm.members = _context.Members.Include(m => m.ForumLists).ToList();
            vm.level = _context.LevelLists.Include(l => l.Members).ToList();
            vm.prodPhoto = forum_prodPhoto();
            vm.productTags = _context.ProductTagLists.Include(t => t.ProductTagDetails).ToList();
            }catch(Exception ex)
            {
                throw new Exception("發生 IOException 異常: " + ex.Message);
            }
              
        }
       
        //顯示每篇文章的trip中剩餘數最少的
        private List<ScheduleList1> ScheduleStock()
        {
            try
            {
                List<ScheduleList1> schedule = new List<ScheduleList1>();
                schedule = ScheduleForum();
                CProductFactory prodFactory = new CProductFactory();
                foreach (var item in schedule)
                {
                    double compare = 1000;
                    int id = 0;
                    double stockRate = 0;
                    foreach (var d in item.trips)
                    {
                        var strStock = prodFactory.TripStock(d.TripId);
                        double r = Convert.ToDouble(strStock.Split('/')[0]);
                        double m = Convert.ToDouble(strStock.Split('/')[1]);

                        if (m - r < compare)
                        {
                            compare = m - r;
                            id = d.TripId;
                        }
                    }
                    item.strStock = prodFactory.TripStock(id);
                    double leave = Convert.ToDouble(item.strStock.Split('/')[0]);
                    double max = Convert.ToDouble(item.strStock.Split('/')[1]);
                    if (leave > max)
                    {
                        leave = max;
                        item.strStock = $"{max}/{max}";
                    }
                    stockRate = leave / max;
                    item.numStock = stockRate;
                }
                var available = schedule.OrderBy(s => s.ForumList.DueDate).Where(s => s.ForumList.DueDate >= DateTime.Now);
                var expired = schedule.OrderBy(s => s.ForumList.DueDate).Where(s => s.ForumList.DueDate < DateTime.Now);
                schedule = available.Concat(expired).ToList();

                return schedule;
            }
            catch(Exception ex) 
            {
                throw new Exception("發生 IOException 異常: " + ex.Message);
            }
            
        }

        //////////////////////////////// /////////////////////////////////MVC/ ////////////////////////////////////////////////////////////////

        int pageSize = 8;
        int itemsPerPage = 0; // 每頁顯示的項目數
        int itemsToSkip = 0;
        public IActionResult ForumList(CForumListViewModel vm, int page = 1)
        {
            List<CForumList_prodPhoto> prodPhotos = new List<CForumList_prodPhoto>();
            ////發文內行程相對應的分類及標籤
            var prodId = _context.ScheduleLists.Select(s => s.Trip.ProductId).Distinct().ToList();
            vm.categories = _context.ProductTagLists
                .Include(t => t.ProductTagDetails)
                .Include(t => t.ProductTagDetails.ProductCategory)
                .Where(t => prodId.Contains((int)t.ProductId))
                .GroupBy(t => t.ProductTagDetails.ProductCategory.ProductCategoryName)
                .Select(g => new CCategoryAndTags
                {
                    category = g.Key,
                    productTags = g.Select(t => new ProductTagDetail
                    {
                        ProductTagDetailsId = (int)t.ProductTagDetailsId,
                        ProductTagDetailsName = t.ProductTagDetails.ProductTagDetailsName,
                    }).ToList()
                })
                .ToList();


            //發文內行程相對應的國家及地區
            vm.regions = _context.ScheduleLists
                .Include(s => s.Trip)
                .Include(s => s.Trip.Product.City)
                .Include(s => s.Trip.Product.City.Country)
                .Where(s => s.Trip.ProductId == s.Trip.Product.ProductId)
                .GroupBy(s => s.Trip.Product.City.Country.Country)
                .Select(g => new CCountryAndCity
                {
                    country = g.Key,
                    citys = g.Select(t => t.Trip.Product.City.City)
                })
                .ToList();

            //vm.schedules = ScheduleForum();
            vm.schedules= ScheduleStock();

            
                var options = new JsonSerializerOptions
                {
                    MaxDepth = 128,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                json = JsonSerializer.Serialize(vm.schedules, options);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDSCHEDULE_INFO, json);
            

            vm.replyList = _context.ReplyLists.ToList();
            vm.members = _context.Members.Include(m => m.ForumLists).ToList();
            vm.level = _context.LevelLists.Include(l => l.Members).ToList();
            vm.prodPhoto = forum_prodPhoto();
            ViewBag.memberId = HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER);

            vm.topTen = _context.ScheduleLists
                .Include(s => s.ForumList)
                .Where(s => s.ForumList.IsPublish == true)
                .Where(s=>s.ForumList.DueDate>=DateTime.Now)
                .GroupBy(s => s.ForumListId)
                 .Select(group => new CForumList_topTen
                 {
                     forumlistid = group.Key,
                     totalPrice = group.Sum(s => s.Trip.UnitPrice),
                     title = group.First().ForumList.Title,
                     prodId = group.Select(t => t.Trip.ProductId).First(),
                     content = group.First().ForumList.Content
                 })
                    .OrderByDescending(item => item.totalPrice)
                    .ToList();


            vm.productTags = _context.ProductTagLists.Include(t => t.ProductTagDetails).ToList();
            int currentPage = page < 1 ? 1 : page;
            vm.pages = vm.schedules.ToPagedList(currentPage, pageSize);
            return View(vm);
        }

        

        //新增文章
        public IActionResult Create()
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGGEDIN_USER))
            {
                TempData[CDictionary.SK_BACK_TO_ACTION] = "Create";
                TempData[CDictionary.SK_BACK_TO_CONTROLLER] = "Forum";
                Task.Delay(3000).Wait();
                return RedirectToAction("Login", "Login");

            }
            ViewBag.memberId = HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER);
            return View();
        }
        [HttpPost]
        public IActionResult Create(CCreatArticleViewModel creatArticle)
        {
            //if (creatArticle.isSave == "儲存草稿")
            //{
                
            //}
            //if (creatArticle.isPublish == "發布")
            //{
            //    creatArticle.forum.IsPublish = true;
            //}
            creatArticle.forum.IsPublish = false;

            //改成到結帳成功後才有發布時間
            //creatArticle.forum.ReleaseDatetime = DateTime.Now;


            _context.Add(creatArticle.forum);
            _context.SaveChanges();

            foreach (int tripId in creatArticle.tripIds)
            {
                if (tripId != 0)
                {
var newSchedule = new ScheduleList
                {
                    ForumListId = creatArticle.forum.ForumListId,
                    TripId = tripId
                };
                _context.Add(newSchedule);
                }
                
            }
            //如果有選形成但沒有選截止日期 先帶入最小日期的前三天
            if(creatArticle.tripIds.Count > 0 && creatArticle.forum.DueDate == null)
            {
                    creatArticle.forum.DueDate = _context.Trips.Where(t => creatArticle.tripIds.Contains(t.TripId)).Min(t => t.Date).Value.AddDays(-3);   
            }
            _context.SaveChanges();
            Task.Delay(3000).Wait();
            if (creatArticle.isSave == "儲存草稿")
            {
                return RedirectToAction("forumList", "Member");
            }
            if (creatArticle.isPublish == "結帳去")
            {
                var routeValues = new RouteValueDictionary
{
    { "ForumListID", creatArticle.forum.ForumListId },
                    {"from",0 }
}; 
                return RedirectToAction("ForumCheckout", "Cart", routeValues);
                //return RedirectToAction("ConfirmOrder", "Cart", new { });
            }
            return RedirectToAction("forumList", "Member");
        }

        //修改文章
        public IActionResult Edit(int? forumlist)
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGGEDIN_USER))
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.memberId_edit = HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER);

            CCreatArticleViewModel vm = new CCreatArticleViewModel();
            vm.forum = _context.ForumLists.Find(forumlist);
            vm.schedule = _context.ScheduleLists.Find(forumlist);
            vm.tripIds = _context.ScheduleLists.Where(s => s.ForumListId == forumlist).Select(s => (int)s.TripId).ToList();

            vm.schedules = _context.ScheduleLists.Include(s => s.Trip).Include(s => s.Trip.Product).Where(s => s.TripId == s.Trip.TripId && s.ForumListId == forumlist).ToList();
            //_context.ScheduleLists.Include(s=>s.ForumList).Include(s=>s.Trip).Include(s=>s.Trip.Product).Where(f=>f.ForumListId == forumlist).ToList(); 
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(CCreatArticleViewModel creatArticle)
        {
            //creatArticle.forum.IsPublish = false;

            //先把原本db該文章的Schedule刪除
            var original_scheduleId = _context.ScheduleLists.Where(s => s.ForumListId == creatArticle.forum.ForumListId).ToList();
            _context.RemoveRange(original_scheduleId);
            _context.SaveChanges();

            //再新增一次該文的Schedule
            foreach (int tripId in creatArticle.tripIds)
            {
                var newSchedule = new ScheduleList
                {
                    ForumListId = creatArticle.forum.ForumListId,
                    TripId = tripId
                };
                _context.Add(newSchedule);
            }

            var origin_forum = _context.ForumLists.Find(creatArticle.forum.ForumListId);
            origin_forum.Content = creatArticle.forum.Content;

            _context.SaveChanges();
            Task.Delay(3000).Wait();
            if (creatArticle.isSave == "儲存草稿")
            {
                return RedirectToAction("forumList", "Member");
            }
            if (creatArticle.isPublish == "結帳去")
            {

                var routeValues = new RouteValueDictionary
                {
                    { "ForumListID", creatArticle.forum.ForumListId },
                                    {"from",0 }
                };

                return RedirectToAction("ForumCheckout", "Cart", routeValues);
            }
            if (creatArticle.isDemo == "DEMO結帳去")
            {
                var routeValues = new RouteValueDictionary
                {
                    { "ForumListID", creatArticle.forum.ForumListId },
                                    {"from",2 }
                };
                var originArticle = _context.ForumLists.Find(creatArticle.forum.ForumListId);
                originArticle.IsPublish = true;
                originArticle.ReleaseDatetime = DateTime.Now;
                originArticle.DueDate = creatArticle.forum.DueDate;
                originArticle.Content = creatArticle.forum.Content;
                //如果使用者沒輸入日期，自動帶入最小日期的前3天
                if (originArticle.DueDate == null)
                {
                    originArticle.DueDate = _context.Trips.Where(t => creatArticle.tripIds.Contains(t.TripId)).Min(t => t.Date).Value.AddDays(-3);
                }
                _context.SaveChanges();
               
                return RedirectToAction("ForumCheckout", "Cart", routeValues);
            }
            return RedirectToAction("forumList", "Member");  
        }


        public IActionResult ArticleView(int? id,int? returnType)
        {
            if (id == null)
            {
                return RedirectToAction("ForumList");
            }
            //將發文跟跟團的ID區隔避免衝突
            try {
                HttpContext.Session.Remove(CDictionary.SK_FORUMLISTID_FOR_PAY);
                HttpContext.Session.Remove(CDictionary.SK_FORUMLISTID_FOR_PAY_JOIN);
                CArticleViewModel vm = new CArticleViewModel();
                vm.likes = _context.LikeLists.Where(l => l.ForumId == id).Include(l => l.Member).ToList();
                vm.replys = _context.ReplyLists.Where(r => r.ForumListId == id).Include(r => r.Member).ToList();
                vm.fforumAddress = _context.ScheduleLists.Include(s => s.Trip.Product).Where(s => s.ForumListId == id).Select(p => p.Trip.Product.Address).ToList();
                ScheduleList1 article = ScheduleStock().FirstOrDefault(s => s.forumListId == id);
                vm.forum = article.ForumList;
                //打開時加入觀看數
                vm.forum.Watches++;
                _context.SaveChanges();
                vm.articleTrips = article.trips;
                var tagDetailsId = article.productTags.Select(t => t.ProductTagDetailsId).ToList();
                vm.trip_Tags = _context.ProductTagDetails
                    .Where(t => tagDetailsId.Contains(t.ProductTagDetailsId))
                    .Select(t => t.ProductTagDetailsName)
                    .ToList();
                var prodInfo = article.trips.Select(t => t.Product).ToList();
                var prodId = prodInfo.Select(t => t.ProductId).ToList();

                vm.img_Paths = forum_prodPhoto().
                    Where(p => prodId.Contains((int)p.prodId))
                    .GroupBy(p => p.prodId)
                    .Select(g => new CArticle_imgs
                    {
                        img_prodId = g.Key,
                        imgPaths = g.Select(path => path.prodPhotoPath).ToList(),

                    })
                 .ToList();
                vm.avgCommentScores = _context.CommentLists
        .Where(c => prodId.Contains((int)c.ProductId)) // 選擇包含在 prodId 中的評論
        .GroupBy(c => c.ProductId) // 按照 ProductId 分組
        .Select(group => new CCommentScore
        {
            commentProdId = group.Key, // 分組的 ProductId
            commentScore = group.Average(c => c.CommentScore) // 計算平均分數
        })
        .ToList();

                //沒登入的情況
                vm.member = null;
                //有登入的情況
                if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGGEDIN_USER))
                {
                    int memId = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER));
                    vm.member = _context.Members.Find(memId);

                }

                if (vm.forum != null)
                {
                    byte[] photo = vm.forum.Member.Photo;
                    if (photo != null)
                    {
                        string base64String = Convert.ToBase64String(photo);
                        ViewBag.PhotoBase64 = "data:image/jpeg;base64," + base64String;
                    }
                };
                //揪團發文回傳0
                if (returnType == 0)
                {
                    vm.returnStatus = returnType;
                }
                else if (returnType == 1)
                {
                    //揪團跟團回傳1
                    vm.returnStatus = 1;
                }
                return View(vm);
            } catch (Exception ex)
            {
                return RedirectToAction("Error", "Error");
            }
            
        }
        /////////////////////////////////////Api/////////////////////////////////
        /////////////////////////////////////發文用/////////////////////////////////
        //選擇商品
        public IActionResult selectTrips(string? keyword)
        {
            if (!string.IsNullOrEmpty(keyword) && keyword != "undefined")
            {
                var filteredTrips = _context.Trips
                    .Where(t => t.Product.StatusId == 1 && t.Product.Discontinued == false && t.ProductId == t.Product.ProductId
                    && t.Product.ProductName.Contains(keyword)
                    && t.Date.Value > DateTime.Now.AddDays(3)
                    && t.OrderDetails.Sum(o=>o.Quantity) < t.MaxNum)
                    .Select(t => new { t.Product.ProductName, t.Product.ProductId })
                    .Distinct().ToList();
                return Json(filteredTrips);
            }
            var trips = _context.Trips
                .Where(t => t.Product.StatusId == 1 && t.Product.Discontinued == false && t.ProductId == t.Product.ProductId
                && t.Date.Value > DateTime.Now.AddDays(3)
                && t.OrderDetails.Sum(o => o.Quantity) < t.MaxNum)
                .Select(t => new { t.Product.ProductName, t.Product.ProductId }).Distinct().ToList();
            return Json(trips);
        }
        //選到的商品的日期
        public IActionResult selectDate(int? id)
        {
            var dates = _context.Trips
                .Where(t => t.ProductId == id && t.Date.Value > DateTime.Now.AddDays(3) && t.OrderDetails.Sum(o => o.Quantity) < t.MaxNum)
                .OrderBy(t => t.Date)
                .Select(t => new { tripDate = t.Date.Value.ToString("yyyy-MM-dd"), price = t.UnitPrice, tripId = t.TripId }).ToList();
            return Json(dates);
        }
        /////////////////////////////////////檢視文章用/////////////////////////////////
        //文章跟團
        public IActionResult Join(int forumId)
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGGEDIN_USER))
            {
                TempData[CDictionary.SK_BACK_TO_ACTION] = "ArticleView";
                TempData[CDictionary.SK_BACK_TO_CONTROLLER] = "Forum";
                TempData[CDictionary.SK_BACK_TO_PARAM] = "id=" + forumId;
                Task.Delay(2000).Wait();
                return RedirectToAction("Login", "Login");
            }
            //有登入的情況下回傳給前面判斷
            return Content("00");
        }
            //文章按讚
            public IActionResult Likes(int forumId,int memId, int status)
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGGEDIN_USER))
            {
                TempData[CDictionary.SK_BACK_TO_ACTION] = "ArticleView";
                TempData[CDictionary.SK_BACK_TO_CONTROLLER] = "Forum";
                TempData[CDictionary.SK_BACK_TO_PARAM] = "id="+forumId;
                Task.Delay(2000).Wait();
                return RedirectToAction("Login", "Login");
            }
            LikeList? like = _context.LikeLists.FirstOrDefault(l=>l.ForumId==forumId && l.MemberId==memId);
            //還沒按過讚
            if(like == null)
            {
                if (status == 0)
                {
                    LikeList addLike = new LikeList();
                    addLike.ForumId = forumId;
                    addLike.MemberId = memId;
                    addLike.IsLike = true;
                    _context.Add(addLike);
                    _context.SaveChanges();
                    var forumLikeCount = _context.LikeLists.Where(l=>l.ForumId==forumId && l.IsLike == true).Count().ToString();
                    return Content(forumLikeCount);
                }
                if (status == 1)
                {
                    LikeList addLike = new LikeList();
                    addLike.ForumId = forumId;
                    addLike.MemberId = memId;
                    addLike.IsLike = false;
                    _context.Add(addLike);
                    _context.SaveChanges();
                    var forumLikeCount = _context.LikeLists.Where(l => l.ForumId == forumId && l.IsLike == true).Count().ToString();
                    return Content(forumLikeCount);
                }
            }
            else
            {
                if (status == 0)
                {
                    like.IsLike=true;
                    _context.Update(like);
                    _context.SaveChanges();
                    var forumLikeCount = _context.LikeLists.Where(l => l.ForumId == forumId && l.IsLike == true).Count().ToString();
                    return Content(forumLikeCount);
                }
                if (status == 1)
                {
                    like.IsLike = false;
                    _context.Update(like);
                    _context.SaveChanges();
                    var forumLikeCount = _context.LikeLists.Where(l => l.ForumId == forumId && l.IsLike==true).Count().ToString();
                    return Content(forumLikeCount);
                }
            }
           
            return NoContent();
        }
        //留言回覆
        public IActionResult ReplyTo(ReplyList reply)
        {
            _context.ReplyLists.Add(reply);
            _context.SaveChanges();
            return Content(reply.ReplyId.ToString().Trim());
        }

        //看該篇發文者的其他文章
        public IActionResult ViewOtherArticle(int memberId)
        {
            List<CForumList_prodPhoto>? prodPhoto = forum_prodPhoto();
            var forumInfos = _context.ScheduleLists.Include(s => s.ForumList).Include(s => s.Trip).Include(s => s.ForumList.Member).Include(s => s.Trip.Product)
                .Where(s => s.ForumList.MemberId == memberId
                && s.ForumList.IsPublish == true)
                .Select(n => new
                {
                    n.ForumListId,
                    n.ForumList.Title,
                    n.ForumList.Watches,
                    releaseDatetime = n.ForumList.ReleaseDatetime.Value.ToString("yyyy-MM-dd"),
                    n.Trip.ProductId
                }).ToList();
            var forumId = forumInfos.Select(n => n.ForumListId).ToList();
            var likes = _context.LikeLists
                .Where(n => forumId.Contains(n.ForumId) && n.IsLike==true)
                .GroupBy(g=>g.ForumId)
                .Select(r => new
                {
                    forumId = r.Key,
                    likeCount = r.Count()
                })
                .ToList();

            var articles = forumInfos.Join(prodPhoto, f => f.ProductId, p => p.prodId,
                (f, p) => new
                {
                    f.ForumListId,
                    f.Title,
                    f.Watches,
                    f.releaseDatetime,
                    p.prodPhotoPath
                }).GroupBy(f => f.ForumListId)
                .Select(g => new
                {
                    ForumListId = g.Key,
                    Title = g.Select(g => g.Title).First(),
                    Watches = g.Select(g => g.Watches).First(),
                    ReleaseDatetime = g.Select(g => g.releaseDatetime).First(),
                    ProdPhotoPath = g.Select(g => g.prodPhotoPath).First(),
                    LikeCount = likes.FirstOrDefault(l => l.forumId == g.Key)?.likeCount ?? 0 // 取得對應的 likeCount
                });
            return Json(articles);
        }
        //saveArticle
        public IActionResult saveArticle(ForumList forum)
        {
            _context.ForumLists.Add(forum);
            _context.SaveChanges();
            return Content("成功儲存草稿");
        }
       

        //////////////////////////// //////////PartialView///// ////////////////////////////////////
        //ArticleView的回覆的框框
        public IActionResult ReplyToDiv(Member userId)
        {
            var member = _context.Members.Find(userId);
            return PartialView(member);
        }
        //ArticleView的該文章的全部回覆
        public IActionResult Replied(int? id)
        {
            CArticleViewModel vm = new CArticleViewModel();
            vm.forum = _context.ForumLists.FirstOrDefault(f => f.IsPublish == true && f.ForumListId == id);
            vm.replys = _context.ReplyLists.Where(r => r.ForumListId == id).Include(r => r.Member).ToList();
            return PartialView(vm);
        }
        
        
        
        



        /////////////////////////////////////揪團主頁用/////////////////////////////////
        string json = null;
        //關鍵字
        public IActionResult KeyWordForForum(string keyword, int page = 1, int pageSize = 8)
        {
            CForumListViewModel filteredForum = new CForumListViewModel();
            filteredForum.schedules = ScheduleStock();
            filteredForum.prodPhoto = forum_prodPhoto();
            filteredForum.replyList = _context.ReplyLists.ToList();
            filteredForum.productTags = _context.ProductTagLists.Include(t => t.ProductTagDetails).ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                filteredForum.schedules = ScheduleStock()
                    .Where(f => f.ForumList.Title.Contains(keyword)
                    || f.ForumList.Member.FirstName.Contains(keyword)
                    || f.ForumList.Member.LastName.Contains(keyword)
                    || f.ForumList.Member.Level.Level.Contains(keyword)
                    || f.ForumList.DueDate.Value.ToString("yyyy/MM/dd").Contains(keyword)
                    || f.trips.Any(trip => trip.Product.City.City.Contains(keyword))
                    || f.trips.Any(trip => trip.Product.Address.Contains(keyword))
                    ).ToList();
                if (filteredForum.schedules.Count() > 0)
                {
                    filteredForum.totalCount = filteredForum.schedules.Count();
                    filteredForum.pageSize = pageSize; // 每頁顯示的項目數
                    filteredForum.currentPage = page < 1 ? 1 : page;
                    itemsToSkip = (page - 1) * pageSize;
                    filteredForum.schedules = filteredForum.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                    return PartialView(filteredForum);
                }
                else
                {
                    filteredForum.totalCount = filteredForum.schedules.Count();
                    filteredForum.pageSize = pageSize; // 每頁顯示的項目數
                    filteredForum.currentPage = page < 1 ? 1 : page;
                    itemsToSkip = (page - 1) * pageSize;
                    filteredForum.schedules = filteredForum.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                    return Content($"<h4><img src={Url.Content("~/icons/icons8-error-96.png")}>沒有符合篩選的項目</h4><input id={"updateTotal"} type={"hidden"} value={"0"}>");
                }

            }
            filteredForum.totalCount = filteredForum.schedules.Count();
            filteredForum.pageSize = pageSize; // 每頁顯示的項目數
            filteredForum.currentPage = page < 1 ? 1 : page;
            itemsToSkip = (page - 1) * pageSize;
            filteredForum.schedules = filteredForum.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
            return PartialView(filteredForum);
        }

        //checkbox
        public IActionResult filteredSchedules(List<int> tagsId, List<string> cities, int page = 1, int pageSize = 8)
        { 
            vm.schedules = ScheduleStock();
            
            //有篩選條件做篩選
            if (tagsId.Count > 0)
            {
                vm.schedules = vm.schedules
                    .Where(schedule => schedule.trips
                    .Any(trip => trip.Product.ProductTagLists
                    .Any(tag => tagsId
                    .Contains((int)tag.ProductTagDetailsId))))
                    .ToList();
                var options = new JsonSerializerOptions
                {MaxDepth = 128,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                json = JsonSerializer.Serialize(vm.schedules, options);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDSCHEDULE_INFO, json);
            }
            if (cities.Count > 0)
            {
                vm.schedules = vm.schedules
                                .Where(s => s.trips.Any(t => cities.Contains(t.Product.City.City))).ToList();
                var options = new JsonSerializerOptions
                {MaxDepth = 128,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                json = JsonSerializer.Serialize(vm.schedules, options);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDSCHEDULE_INFO, json);
            }
            if (tagsId.Count == 0 && cities.Count == 0)
            {
                vm.schedules = vm.schedules;
                var options = new JsonSerializerOptions
                {
                    MaxDepth = 128,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                json = JsonSerializer.Serialize(vm.schedules, options);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDSCHEDULE_INFO, json);
            }
                if (vm.schedules.Count == 0)
            {
                return Content($"<h4><img src={Url.Content("~/icons/icons8-error-96.png")}>沒有符合篩選的項目</h4><input id={"updateTotal"} type={"hidden"} value={"0"}>");
            }
            forumInfos();
            vm.pageSize = pageSize; // 每頁顯示的項目數
            vm.currentPage = page < 1 ? 1 : page;
            itemsToSkip = (page - 1) * pageSize;
            vm.totalCount = vm.schedules.Count();
            vm.schedules = vm.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
            return PartialView(vm);
        }
        //排序
        
        public IActionResult filteredBySort(string sortType, int page = 1, int pageSize = 8)
        {
            ViewBag.sortType = sortType;
            var options = new JsonSerializerOptions
            {
                MaxDepth = 128,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_FILETREDSCHEDULE_INFO))
            {
                vm.schedules = ScheduleStock();
               
                json = JsonSerializer.Serialize(vm.schedules, options);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDSCHEDULE_INFO, json);

            }

            json = HttpContext.Session.GetString(CDictionary.SK_FILETREDSCHEDULE_INFO);
            vm.schedules = JsonSerializer.Deserialize<List<ScheduleList1>>(json,options);

           
            //依發文時間遠到近
            if (sortType == "發起時間")
            {
                    forumInfos();
                    vm.schedules = vm.schedules.OrderByDescending(s => s.ForumList.ReleaseDatetime).ToList();
                    vm.pageSize = pageSize; // 每頁顯示的項目數
                    vm.currentPage = page < 1 ? 1 : page;
                    itemsToSkip = (page - 1) * pageSize;
                    vm.totalCount = vm.schedules.Count();
                    vm.schedules = vm.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                    return PartialView(vm);
            }
            //依回覆數
            if (sortType == "回覆數")
            {
                forumInfos();
                vm.schedules = vm.schedules.OrderByDescending(s => s.ForumList.ReplyLists.Count).ToList();
               vm.pageSize = pageSize; // 每頁顯示的項目數
                vm.currentPage = page < 1 ? 1 : page;
                itemsToSkip = (page - 1) * pageSize;
                vm.totalCount = vm.schedules.Count();
                vm.schedules = vm.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                return PartialView(vm);
            }
            //依截團日期
            if (sortType == "截團日期(近到遠)")
            {
                forumInfos();
                //var available = vm.schedules.OrderBy(s => s.ForumList.ReleaseDatetime.Value).Where(s => s.ForumList.DueDate >= DateTime.Now);
                //var expired = vm.schedules.OrderBy(s => s.ForumList.ReleaseDatetime.Value).Where(s => s.ForumList.DueDate < DateTime.Now);
                //vm.schedules = vm.schedules;
                vm.pageSize = pageSize; // 每頁顯示的項目數
                vm.currentPage = page < 1 ? 1 : page;
                itemsToSkip = (page - 1) * pageSize;
                vm.totalCount = vm.schedules.Count();
                vm.schedules = vm.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                return PartialView(vm);
            }
            //依剩餘名額
            if (sortType == "剩餘名額")
            {
                forumInfos();
                vm.schedules = vm.schedules.OrderByDescending(s =>s.numStock).ToList();
                vm.pageSize = pageSize; // 每頁顯示的項目數
                vm.currentPage = page < 1 ? 1 : page;
                itemsToSkip = (page - 1) * pageSize;
                vm.totalCount = vm.schedules.Count();
                vm.schedules = vm.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                return PartialView(vm);
            }
            return PartialView(vm);
        }

        




        //ForumList的篩選欄(地區)
        public IActionResult Region()
        {
            //發文內行程相對應的國家及地區
            vm.regions = _context.ScheduleLists
                .Include(s => s.Trip)
                .Include(s => s.Trip.Product.City)
                .Include(s => s.Trip.Product.City.Country)
                .Where(s => s.Trip.ProductId == s.Trip.Product.ProductId)
                .GroupBy(s => s.Trip.Product.City.Country.Country)
                .Select(g => new CCountryAndCity
                {
                    country = g.Key,
                    citys = g.Select(t => t.Trip.Product.City.City)
                })
                .ToList();
            vm.schedules = ScheduleStock();

            return PartialView(vm);
        }
        //ForumList的篩選欄(地區)
        public IActionResult Category()
        {
            ////發文內行程相對應的分類及標籤
            var prodId = _context.ScheduleLists.Select(s => s.Trip.ProductId).Distinct().ToList();
            vm.categories = _context.ProductTagLists
                .Include(t => t.ProductTagDetails)
                .Include(t => t.ProductTagDetails.ProductCategory)
                .Where(t => prodId.Contains((int)t.ProductId))
                .GroupBy(t => t.ProductTagDetails.ProductCategory.ProductCategoryName)
                 .Select(g => new CCategoryAndTags
                 {
                     category = g.Key,
                     productTags = g.Select(t => new ProductTagDetail
                     {
                         ProductTagDetailsId = (int)t.ProductTagDetailsId,
                         ProductTagDetailsName = t.ProductTagDetails.ProductTagDetailsName,
                     }).ToList()
                 })
                .ToList();
            vm.schedules = ScheduleStock();

            return PartialView(vm);
        }
    }
}
