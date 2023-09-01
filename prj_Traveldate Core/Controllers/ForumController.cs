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
            List<CForumList_prodPhoto> prod_photo = _context.ProductPhotoLists.Where(p => tripId.Contains((int)p.ProductId)).Select(p => new CForumList_prodPhoto
            {
                prodId = (int)p.ProductId,
                prodPhotoPath = p.ImagePath
            }).ToList();
            return prod_photo;
        }
        //要顯示文章的主要資料
        public List<ScheduleList1> ScheduleForum()
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
                })
                .ToList();
            return data;
        }
        //要顯示文章的其他資料
        private void forumInfos()
        {
            vm.replyList = _context.ReplyLists.ToList();
            vm.members = _context.Members.Include(m => m.ForumLists).ToList();
            vm.level = _context.LevelLists.Include(l => l.Members).ToList();
            vm.prodPhoto = forum_prodPhoto();
            vm.productTags = _context.ProductTagLists.Include(t => t.ProductTagDetails).ToList();
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
                    tags = g.Select(t => t.ProductTagDetails.ProductTagDetailsName).ToList()
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

            vm.schedules = ScheduleForum();
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_FILETREDSCHEDULE_INFO))
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                json = JsonSerializer.Serialize(vm.schedules, options);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDPRODUCTS_INFO, json);
            }

            vm.replyList = _context.ReplyLists.ToList();
            vm.members = _context.Members.Include(m => m.ForumLists).ToList();
            vm.level = _context.LevelLists.Include(l => l.Members).ToList();
            vm.prodPhoto = forum_prodPhoto();
            ViewBag.memberId = HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_USER);

            vm.topTen = _context.ScheduleLists
                .Include(s => s.ForumList)
                .Where(s => s.ForumList.IsPublish == true)
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
            if (creatArticle.isSave == "儲存草稿")
            {
                creatArticle.forum.IsPublish = false;
            }
            if (creatArticle.isPublish == "發布")
            {
                creatArticle.forum.IsPublish = true;
            }
            creatArticle.forum.ReleaseDatetime = DateTime.Now;
            _context.Add(creatArticle.forum);
            _context.SaveChanges();

            foreach (int tripId in creatArticle.tripIds)
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
            vm.tripIds = _context.ScheduleLists.Where(s => s.ForumListId == forumlist).Select(s => (int)s.TripId).ToList();

            vm.schedules = _context.ScheduleLists.Include(s => s.Trip).Include(s => s.Trip.Product).Where(s => s.TripId == s.Trip.TripId && s.ForumListId == forumlist).ToList();
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
                .Where(s => s.ForumList.MemberId == memberId)
                .Select(n => new
                {
                    n.ForumListId,
                    n.ForumList.Title,
                    n.ForumList.Watches,
                    n.ForumList.Likes,
                    releaseDatetime = n.ForumList.ReleaseDatetime.Value.ToString("yyyy-MM-dd"),
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
                }).GroupBy(f => f.ForumListId)
                .Select(g => new
                {
                    ForumListId = g.Key,
                    Title = g.Select(g => g.Title).First(),
                    Watches = g.Select(g => g.Watches).First(),
                    Likes = g.Select(g => g.Likes).First(),
                    ReleaseDatetime = g.Select(g => g.releaseDatetime).First(),
                    ProdPhotoPath = g.Select(g => g.prodPhotoPath).First()
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
        public IActionResult KeyWordForForum(string keyword)
        {
            CForumListViewModel filteredForum = new CForumListViewModel();
            filteredForum.schedules = ScheduleForum();
            filteredForum.prodPhoto = forum_prodPhoto();
            filteredForum.replyList = _context.ReplyLists.ToList();
            filteredForum.productTags = _context.ProductTagLists.Include(t => t.ProductTagDetails).ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                filteredForum.schedules = ScheduleForum()
                    .Where(f => f.ForumList.Title.Contains(keyword)
                    || f.ForumList.Member.FirstName.Contains(keyword)
                    || f.ForumList.Member.LastName.Contains(keyword)
                    || f.ForumList.Member.Level.Level.Contains(keyword)
                    || f.ForumList.DueDate.Value.ToString("yyyy/MM/dd").Contains(keyword)
                    || f.trips.Any(trip => trip.Product.City.City.Contains(keyword))
                    ).ToList();
                if (filteredForum.schedules.Count() > 0)
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

        //checkbox
        public IActionResult filteredSchedules(List<string> tags, List<string> cities, int page = 1, int pageSize = 8)
        { 
            vm.schedules = ScheduleForum();
         
            //有篩選條件做篩選
            if (tags.Count > 0)
            {
                vm.schedules = vm.schedules
                    .Where(schedule => schedule.trips.Any(trip => trip.Product.ProductTagLists.Any(tag => tags.Contains(tag.ProductTagDetails.ProductTagDetailsName))))
                    .ToList();
                var options = new JsonSerializerOptions
                {
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
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                json = JsonSerializer.Serialize(vm.schedules, options);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDSCHEDULE_INFO, json);
            }
            if (tags.Count == 0 && cities.Count == 0)
            {
                vm.schedules = vm.schedules;
                var options = new JsonSerializerOptions
                {
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
                ReferenceHandler = ReferenceHandler.Preserve
            };
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_FILETREDSCHEDULE_INFO))
            {
                vm.schedules = ScheduleForum();
               
                json = JsonSerializer.Serialize(vm.schedules, options);
                HttpContext.Session.SetString(CDictionary.SK_FILETREDSCHEDULE_INFO, json);

            }

            json = HttpContext.Session.GetString(CDictionary.SK_FILETREDSCHEDULE_INFO);
            vm.schedules = JsonSerializer.Deserialize<List<ScheduleList1>>(json,options);
            //依發文時間(近到遠)
            if(sortType == "sortByRelTime")
            {
                forumInfos();
                vm.schedules = vm.schedules.OrderBy(s => s.ForumList.ReleaseDatetime.Value).ToList();
                vm.pageSize = pageSize; // 每頁顯示的項目數
                vm.currentPage = page < 1 ? 1 : page;
                itemsToSkip = (page - 1) * pageSize;
                vm.totalCount = vm.schedules.Count();
                vm.schedules = vm.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                return PartialView(vm);
            }
            //依回覆數
            if (sortType == "sortByReply")
            {
                forumInfos();
               vm.pageSize = pageSize; // 每頁顯示的項目數
                vm.currentPage = page < 1 ? 1 : page;
                itemsToSkip = (page - 1) * pageSize;
                vm.totalCount = vm.schedules.Count();
                vm.schedules = vm.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                return PartialView(vm);
            }
            //依截團日期
            if (sortType == "sortByDueDate")
            {
                forumInfos();
                vm.schedules = vm.schedules.OrderBy(s => s.ForumList.DueDate.Value).ToList();
                vm.pageSize = pageSize; // 每頁顯示的項目數
                vm.currentPage = page < 1 ? 1 : page;
                itemsToSkip = (page - 1) * pageSize;
                vm.totalCount = vm.schedules.Count();
                vm.schedules = vm.schedules.Skip(itemsToSkip).Take(pageSize).ToList();
                return PartialView(vm);
            }
            //依剩餘名額
            if (sortType == "sortByStock")
            {
                forumInfos();
                vm.schedules = vm.schedules.OrderBy(s => s.ForumList.ReleaseDatetime.Value).ToList();
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
            vm.schedules = ScheduleForum();

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
                    tags = g.Select(t => t.ProductTagDetails.ProductTagDetailsName).ToList(),

                })
                .ToList();
            vm.schedules = ScheduleForum();

            return PartialView(vm);
        }

    }
}
