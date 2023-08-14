using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

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
            vm.forumList= _context.ForumLists.ToList();
            vm.replyList = _context.ReplyLists.ToList();
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
            vm.replys = _context.ReplyLists.Where(r => r.ForumListId == id).Include(r=>r.Member).ToList(); ;
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
        public IActionResult selectTrips()
        {
            List<string> trips = new List<string>();
            //if (string.IsNullOrEmpty(txtKeyword))
            //{
               
               
            //    return Json(trips);
            //}
            //trips = _context.Trips
            //        .Where(t => t.Product.StatusId == 1 && t.Product.Discontinued == false && t.ProductId == t.Product.ProductId)
            //        .Where(t => t.Product.ProductName.ToUpper().StartsWith(txtKeyword.ToUpper()))
            //        .Select(t => t.Product.ProductName).Distinct().ToList();
            trips = _context.Trips.Where(t => t.Product.StatusId == 1 && t.Product.Discontinued == false && t.ProductId == t.Product.ProductId).Select(t => t.Product.ProductName).Distinct().ToList();
            return Json(trips);
        }


        //////////////////////////// //////////////////////////////////PartialComponenet///// //////////////////////////////////////////////////////////////
        //回覆的框框
        public IActionResult ReplyToDiv(Member userId) 
        {
            var member = _context.Members.Find(userId);
            return PartialView(member);
        }
        //該文章的全部回覆
        public IActionResult Replied(int? id)
        {
            CArticleViewModel vm = new CArticleViewModel();
            vm.forum = _context.ForumLists.Find(id);
            vm.replys = _context.ReplyLists.Where(r => r.ForumListId == id).Include(r => r.Member).ToList(); 
            return PartialView(vm);
        }
    }
}
