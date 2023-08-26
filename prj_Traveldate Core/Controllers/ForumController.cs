using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.Drawing;

namespace prj_Traveldate_Core.Controllers
{
    public class ForumController : Controller
    {
        
        private TraveldateContext _context;
        public ForumController(TraveldateContext context)
        {
            _context = context;
        }
     //////////////////////////////// /////////////////////////////////MVC/ ////////////////////////////////////////////////////////////////
        public IActionResult ForumList(CForumListViewModel vm)
        {
            CFilteredProductFactory factory = new CFilteredProductFactory();
            List<CForumList_prodPhoto> prodPhotos = new List<CForumList_prodPhoto>();
           vm.regions = factory.qureyFilterCountry();
            vm.forumList = _context.ArticlePhotos.Include(photo=>photo.ForumList).ToList();
            vm.schedules = _context.ScheduleLists.Include(s => s.ForumList).Include(s=>s.Trip).Include(s=>s.ForumList.Member).Include(s => s.Trip.Product).ToList();
            vm.replyList = _context.ReplyLists.ToList();
            vm.members = _context.Members.Include(m=>m.ForumLists).ToList();
            vm.level = _context.LevelLists.Include(l=>l.Members).ToList();
            vm.categories = factory.qureyFilterCategories();
            //vm.schedulesForProd = _context.ScheduleLists.Include(s => s.ForumList).Include(s => s.Trip).Include(s => s.Trip.Product).ToList();
            var tripId = _context.ScheduleLists.Where(s => s.TripId == s.Trip.TripId).Select(s=>s.Trip.Product.ProductId).ToList();
            var prod_photo= _context.ProductPhotoLists.Where(p=>tripId.Contains((int)p.ProductId)).Select(p=>new CForumList_prodPhoto
            {
                prodId = (int)p.ProductId,
                prodPhotoPath = p.ImagePath
            }).ToList();
            prodPhotos.AddRange(prod_photo);
            vm.prodPhoto = prodPhotos;
            //TODO 把Schedule的prodId跟prodPhotoId連結並放到畫面上
            return View(vm);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CCreatArticleViewModel creatArticle)
        {
            if (creatArticle.isSave== "儲存草稿")
            {
                creatArticle.forum.IsPublish = false;
            }
            if (creatArticle.isPublish == "結帳成功")
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
            return View();
        }

        public IActionResult ArticleView(int? id)
        {
           if(id == null)
            {
                return RedirectToAction("ForumList");
            }
            CArticleViewModel vm = new CArticleViewModel();
            vm.forum = _context.ForumLists.Find(id);
            vm.replys = _context.ReplyLists.Where(r => r.ForumListId == id).Include(r => r.Member).ToList();
            vm.member = _context.Members.Find(6);
            vm.fforumAddress = _context.ScheduleLists.Include(s => s.Trip.Product).Where(s => s.ForumListId == id).Select(p => p.Trip.Product.Address).ToList();
           
            return View(vm);
        }
        //////////////////////////////// /////////////////////////////////Api/ ////////////////////////////////////////////////////////////////
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
        //發文選擇商品
        public IActionResult selectTrips(string? keyword)
        {
            if (!string.IsNullOrEmpty(keyword) && keyword!="undefined")
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
        //選到的發文商品的日期
        public IActionResult selectDate(int? id)
        {
            var dates = _context.Trips.Where(t=>t.ProductId==id && t.Date.Value > DateTime.Now.AddDays(3)).OrderBy(t=>t.Date).Select(t=>new { tripDate = t.Date.Value.ToString("yyyy-MM-dd"),price =t.UnitPrice,tripId = t.TripId }).ToList();
            return Json(dates);
        }

            //////////////////////////// //////////////////////////////////PartialComponenet///// //////////////////////////////////////////////////////////////
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
            vm.forum = _context.ForumLists.Find(id);
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
