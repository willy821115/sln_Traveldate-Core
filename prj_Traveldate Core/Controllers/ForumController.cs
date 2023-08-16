using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
           vm.regions = factory.qureyFilterCountry();
           vm.forumList = _context.ForumLists.ToList();
            vm.replyList = _context.ReplyLists.ToList();
            vm.members = _context.Members.Include(m=>m.ForumLists).ToList();
            vm.level = _context.LevelLists.Include(l=>l.Members).ToList();
            //item.photo = db.ProductPhotoLists.Where(p => item.productID == p.ProductId).Select(p => p.Photo).FirstOrDefault();
            
            var ddd = _context.ScheduleLists.Where(s => s.TripId == s.Trip.TripId).Select(s=>s.Trip.Product.ProductId).ToList();
            vm.prodPhoto = _context.ProductPhotoLists.Where(p=>ddd.Contains((int)p.ProductId)).ToList();
            return View(vm);
        }
        public IActionResult Create()
        {
            List<Trip> trips= _context.Trips.Where(t => t.Product.StatusId == 1 && t.Product.Discontinued == false && t.ProductId == t.Product.ProductId).ToList();
            return View(trips);
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
                var filterdtrips = _context.Trips
                    .Where(t => t.Product.StatusId == 1 && t.Product.Discontinued == false && t.ProductId == t.Product.ProductId)
                    .Where(t=>t.Product.ProductName.Contains(keyword))
                    .Select(t => new { t.Product.ProductName, t.Product.ProductId })
                    .Distinct().ToList();
                return Json(filterdtrips);
            }
            var trips = _context.Trips
                .Where(t => t.Product.StatusId == 1 && t.Product.Discontinued == false && t.ProductId == t.Product.ProductId)
                .Select(t => new { t.Product.ProductName, t.Product.ProductId }).Distinct().ToList();
            return Json(trips);
        }
        //選到的發文商品的日期
        public IActionResult selectDate(int? id)
        {
            var dates = _context.Trips.Where(t=>t.ProductId==id).Select(t=>t.Date.Value.ToString("yyyy/MM/dd")).ToList();
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
    }
}
