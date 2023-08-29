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

namespace prj_Traveldate_Core.Controllers
{
    public class ForumController : Controller
    {
        
        private TraveldateContext _context;
        public ForumController(TraveldateContext context)
        {
            _context = context;
        }
        List<CForumList_prodPhoto> forum_prodPhoto()
        {

            var tripId = _context.ScheduleLists.Where(s => s.TripId == s.Trip.TripId).Select(s => s.Trip.Product.ProductId).ToList();
            List<CForumList_prodPhoto> prod_photo = _context.ProductPhotoLists.Where(p => tripId.Contains((int)p.ProductId)).Select(p => new CForumList_prodPhoto
            {
                prodId = (int)p.ProductId,
                prodPhotoPath = p.ImagePath
            }).ToList();
            return prod_photo;
        }
      
     //////////////////////////////// /////////////////////////////////MVC/ ////////////////////////////////////////////////////////////////
     public List<ScheduleList> ScheduleForum()
        {
            List<ScheduleList> data = _context.ScheduleLists
                .Include(s => s.ForumList)
                .Include(s => s.Trip)
                .Include(s => s.ForumList.Member)
                .Include(s => s.Trip.Product)
                .Include(s=>s.ForumList.Member.Level)
                .Where(s => s.ForumList.IsPublish == true)
                .GroupBy(g => g.ForumListId)
                .Select(g => new ScheduleList
                {
                    ForumListId = g.Key,
                    Trip = g.First().Trip,
                    ForumList = g.First().ForumList// 第一個 Trip
                })
                .ToList();
            return data;
        }
        public IActionResult ForumList(CForumListViewModel vm)
        { 
            
            List<CForumList_prodPhoto> prodPhotos = new List<CForumList_prodPhoto>();
           //發文內行程相對應的分類及標籤
            var prodId = _context.ScheduleLists.Select(s => s.Trip.ProductId).Distinct().ToList();
            vm.categories = _context.ProductTagLists
                .Include(t => t.ProductTagDetails)
                .Include(t => t.ProductTagDetails.ProductCategory)
                .Where(t=>prodId.Contains((int)t.ProductId))
                .GroupBy(t => t.ProductTagDetails.ProductCategory.ProductCategoryName)
                .Select(g=>new CCategoryAndTags
                {
                    category = g.Key,
                    tags = g.Select(t=>t.ProductTagDetails.ProductTagDetailsName).ToList()
                })
                .ToList();


            //發文內行程相對應的國家及地區
            vm.regions = _context.ScheduleLists
                .Include(s=>s.Trip)
                .Include(s=>s.Trip.Product.City)
                .Include(s=>s.Trip.Product.City.Country)
                .Where(s=>s.Trip.ProductId == s.Trip.Product.ProductId)
                .GroupBy(s=>s.Trip.Product.City.Country.Country)
                .Select(g=>new CCountryAndCity 
                { 
                    country = g.Key,
                    citys = g.Select(t=>t.Trip.Product.City.City)
                })
                .ToList();

            //vm.forumList = _context.ArticlePhotos.Include(photo=>photo.ForumList).ToList();
            vm.schedules = ScheduleForum();
            vm.replyList = _context.ReplyLists.ToList();
            vm.members = _context.Members.Include(m=>m.ForumLists).ToList();
            vm.level = _context.LevelLists.Include(l=>l.Members).ToList();

            //vm.schedulesForProd = _context.ScheduleLists.Include(s => s.ForumList).Include(s => s.Trip).Include(s => s.Trip.Product).ToList();

           
            vm.prodPhoto = forum_prodPhoto();
            ViewBag.memberId = HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER);

            var topTenArticle = _context.ScheduleLists
                .Where(s => s.ForumList.IsPublish == true)
                .GroupBy(s => s.ForumListId)
                 .Select(group => new CForumList_topTen
                 {
                        forumlistid = group.Key,
                        totalPrice = group.Sum(s => s.Trip.UnitPrice),
                        title = group.First().ForumList.Title,
                        prodId = group.Select(t => t.Trip.ProductId).First()
                    })
                    .OrderByDescending(item => item.totalPrice)
                    .ToList();

            vm.topTen = topTenArticle; 
            vm.productTags = _context.ProductTagLists.Include(t=>t.ProductTagDetails).ToList();
            return View(vm);
        }
        //新增文章
        public IActionResult Create()
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGGEDIN_USER))
            {
                TempData[CDictionary.SK_BACK_TO_ACTION]= "Create";
                TempData[CDictionary.SK_BACK_TO_CONTROLLER]= "Forum";
                Task.Delay(3000).Wait();
                return RedirectToAction("Login", "Login");
                
            }
            ViewBag.memberId = HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER);
            return View();
        }
        [HttpPost]
        public IActionResult Create(CCreatArticleViewModel creatArticle)
        {
            if (creatArticle.isSave== "儲存草稿")
            {
                creatArticle.forum.IsPublish = false;
            }
            if (creatArticle.isPublish == "發布")
            {
                creatArticle.forum.IsPublish=true;
            }
            creatArticle.forum.ReleaseDatetime = DateTime.Now; 
            _context.Add(creatArticle.forum);
            _context.SaveChanges();
           
            foreach(int tripId in creatArticle.tripIds)
            {
                var newSchedule = new ScheduleList
                {
                    ForumListId = creatArticle.forum.ForumListId,
                    TripId = tripId
                };
                _context.Add(newSchedule);
            }
            
            _context.SaveChanges();
            Task.Delay(3000).Wait();
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
            vm.tripIds = _context.ScheduleLists.Where(s=>s.ForumListId == forumlist).Select(s=>(int)s.TripId).ToList();
            
           vm.schedules = _context.ScheduleLists.Include(s => s.Trip).Include(s=>s.Trip.Product).Where(s=>s.TripId==s.Trip.TripId && s.ForumListId== forumlist).ToList();
            //_context.ScheduleLists.Include(s=>s.ForumList).Include(s=>s.Trip).Include(s=>s.Trip.Product).Where(f=>f.ForumListId == forumlist).ToList(); 
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(CCreatArticleViewModel article)
        {
            if (article.isSave == "儲存草稿")
            {
                article.forum.IsPublish = false;
            }
            if (article.isPublish == "發布")
            {
                article.forum.IsPublish = true;
            }
            article.forum.ReleaseDatetime = DateTime.Now;
            _context.Update(article.forum);
            _context.SaveChanges();

            foreach (int tripId in article.tripIds)
            {
                var newSchedule = new ScheduleList
                {
                    ForumListId = article.forum.ForumListId,
                    TripId = tripId
                };
                _context.Update(newSchedule);
            }

            _context.SaveChanges();
            Task.Delay(3000).Wait();
            return RedirectToAction("Index", "Member");

            
        }


        public IActionResult ArticleView(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ForumList");
            }
      

            CArticleViewModel vm = new CArticleViewModel();
            vm.forum = _context.ForumLists.Include(f => f.Member).FirstOrDefault(f => f.ForumListId == id);
            vm.replys = _context.ReplyLists.Where(r => r.ForumListId == id).Include(r => r.Member).ToList();
            vm.fforumAddress = _context.ScheduleLists.Include(s => s.Trip.Product).Where(s => s.ForumListId == id).Select(p => p.Trip.Product.Address).ToList();
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
            return View(vm);
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
                    && t.Date.Value > DateTime.Now.AddDays(3))
                    .Select(t => new { t.Product.ProductName, t.Product.ProductId })
                    .Distinct().ToList();
                return Json(filteredTrips);
            }
            var trips = _context.Trips
                .Where(t => t.Product.StatusId == 1 && t.Product.Discontinued == false && t.ProductId == t.Product.ProductId
                && t.Date.Value > DateTime.Now.AddDays(3))
                .Select(t => new { t.Product.ProductName, t.Product.ProductId }).Distinct().ToList();
            return Json(trips);
        }
        //選到的商品的日期
        public IActionResult selectDate(int? id)
        {
            var dates = _context.Trips.Where(t => t.ProductId == id && t.Date.Value > DateTime.Now.AddDays(3)).OrderBy(t => t.Date).Select(t => new { tripDate = t.Date.Value.ToString("yyyy-MM-dd"), price = t.UnitPrice, tripId = t.TripId }).ToList();
            return Json(dates);
        }
        /////////////////////////////////////檢視文章用/////////////////////////////////
        //文章按讚
        public IActionResult Likes(int id, int status)
        {
            ForumList? forum = _context.ForumLists.Find(id);
            if (status == 0)
            {
                forum.Likes++;
                _context.Update(forum);
                _context.SaveChanges();
                return Content(forum.Likes.ToString());
            }
            if (status == 1)
            {
                forum.Likes--;
                _context.Update(forum);
                _context.SaveChanges();
                return Content(forum.Likes.ToString());
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
                .Where(s=>s.ForumList.MemberId==memberId)
                .Select(n => new
                {
                    n.ForumListId,
                    n.ForumList.Title,
                    n.ForumList.Watches,
                    n.ForumList.Likes,
                    releaseDatetime =n.ForumList.ReleaseDatetime.Value.ToString("yyyy-MM-dd"),
                    n.Trip.ProductId
                }).ToList();
            var articles = forumInfos.Join(prodPhoto, f => f.ProductId, p => p.prodId,
                (f, p) => new
                {
                    f.ForumListId,
                    f.Title,
                    f.Watches,
                    f.Likes,
                    f.releaseDatetime,
                    p.prodPhotoPath
                }).GroupBy(f=>f.ForumListId)
                .Select(g => new
                {
                    ForumListId = g.Key,
                    Title = g.Select(g=>g.Title).First(),
                    Watches = g.Select(g=>g.Watches).First(),
                    Likes = g.Select(g => g.Likes).First(),
                    ReleaseDatetime = g.Select(g=>g.releaseDatetime).First(),
                    ProdPhotoPath = g.Select(g=>g.prodPhotoPath).First()
                });
            return Json(articles);
        }
        /////////////////////////////////////揪團主頁用/////////////////////////////////
        public IActionResult KeyWordForForum(string keyword)
        {
            CForumListViewModel filteredForum = new CForumListViewModel();
            filteredForum.schedules = ScheduleForum();
            filteredForum.prodPhoto = forum_prodPhoto();
            filteredForum.replyList = _context.ReplyLists.ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                filteredForum.schedules = ScheduleForum().Where(f => f.ForumList.Title.Contains(keyword)).ToList();
                if(filteredForum.schedules.Count()>0)
                {
                    return PartialView(filteredForum);
                }
                else
                {
                    return Content($"<h4><img src={Url.Content("~/icons/icons8-error-96.png")}>沒有符合篩選的項目</h4><input id={"updateTotal"} type={"hidden"} value={"0"}>");
                }
                
            }
            return PartialView(filteredForum);
        } 

            //////////////////////////// //////////PartialComponenet///// ////////////////////////////////////
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
            vm.forum = _context.ForumLists.FirstOrDefault(f=>f.IsPublish==true && f.ForumListId == id);
            vm.replys = _context.ReplyLists.Where(r => r.ForumListId == id).Include(r => r.Member).ToList(); 
            return PartialView(vm);
        }
        //ForumList的篩選欄(地區)
        public IActionResult Region()
        {
            CFilteredProductFactory factory = new CFilteredProductFactory();
            List<CCountryAndCity> regions = factory.qureyFilterCountry();
            return PartialView(regions);
        }
        //ForumList的篩選欄(地區)
        public IActionResult Category()
        {
            CFilteredProductFactory factory = new CFilteredProductFactory();
            List<CCategoryAndTags> categories = factory.qureyFilterCategories();
            return PartialView(categories);
        }
        //CreateArticle
        public IActionResult saveArticle(ForumList forum)
        {
            _context.ForumLists.Add(forum);
            _context.SaveChanges();
            return Content("成功儲存草稿");
        }
        public IActionResult tee() 
        {
            return View();
        }
    }
}
