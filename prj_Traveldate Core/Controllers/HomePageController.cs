using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prj_Traveldate_Core.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult HomePage()
        {
            CHomePresent list = new CHomePresent();

            TraveldateContext db = new TraveldateContext();

            var popular = from od in db.OrderDetails
                          where (bool)!od.Trip.Product.Discontinued&&od.Trip.Product.StatusId==1
                           group od by od.Trip.Product into g
                          orderby g.Select(o => o.Quantity).Sum() descending
                          select new { productId = g.Key.ProductId, productName = g.Key.ProductName, unitPrice = g.Min(od => od.Trip.UnitPrice) };

            List<CHomeViewModel> popularList = new List<CHomeViewModel>();
            try
            {

                foreach (var p in popular.Take(6).ToList())
                {
                    CHomeViewModel item = new CHomeViewModel();
                    item.productId = Convert.ToInt32(p.productId);
                    item.productName = p.productName;
                    item.unitPrice = (decimal)p.unitPrice;

                    var commentScore = db.CommentLists.Where(c => c.ProductId == p.productId).Select(c => c.CommentScore).Average();
                    var imagePath = db.ProductPhotoLists.Where(c => c.ProductId == p.productId).Select(c => c.ImagePath).FirstOrDefault();

                    item.ImagePath = imagePath;
                    item.commentScore = commentScore;
                    item.commentScoreString = string.Format("{0:F1}", commentScore);
                    popularList.Add(item);
                }

                list.popularList = popularList;

                var comment = from c in db.CommentLists
                              where (bool)!c.Product.Discontinued && c.Product.StatusId == 1
                              group c by c.Product into g
                              orderby g.Select(c => c.CommentScore).Average() descending
                              select new { productId = g.Key.ProductId, productName = g.Key.ProductName, commentScore = g.Select(c => c.CommentScore).Average() };

                List<CHomeViewModel> commentList = new List<CHomeViewModel>();
                //todo本機登入會報錯的地方先註解
                foreach (var p in comment.Take(8).ToList())
                {
                    CHomeViewModel item = new CHomeViewModel();
                    item.productId = p.productId;
                    item.productName = p.productName;
                    item.commentScore = (double)p.commentScore;
                    item.commentScoreString = string.Format("{0:F1}", p.commentScore);
                    var unitprice = db.Trips.Where(c => c.ProductId == p.productId).Min(c => c.UnitPrice);
                    var imagePath = db.ProductPhotoLists.Where(c => c.ProductId == p.productId).Select(c => c.ImagePath).FirstOrDefault();

                    item.ImagePath = imagePath;
                    item.unitPrice = (decimal)unitprice;

                    commentList.Add(item);
                }
                list.commentList = commentList;

                var discount = from od in db.Trips
                               where (bool)!od.Product.Discontinued && od.Product.StatusId == 1
                               group od by od.Product into g
                               orderby g.Max(o => o.Discount) descending
                               select new { productId = g.Key.ProductId, productName = g.Key.ProductName, unitPrice = g.Min(od => od.UnitPrice) };

                List<CHomeViewModel> discountList = new List<CHomeViewModel>();
                foreach (var p in discount.Take(8).ToList())
                {
                    CHomeViewModel item = new CHomeViewModel();
                    item.productId = Convert.ToInt32(p.productId);
                    item.productName = p.productName;
                    item.unitPrice = (decimal)p.unitPrice;

                    var commentScore = db.CommentLists.Where(c => c.ProductId == p.productId).Select(c => c.CommentScore).Average();
                    var imagePath = db.ProductPhotoLists.Where(c => c.ProductId == p.productId).Select(c => c.ImagePath).FirstOrDefault();

                    item.ImagePath = imagePath;
                    item.commentScore = commentScore;
                    item.commentScoreString = string.Format("{0:F1}", commentScore);
                    discountList.Add(item);
                }

                list.discountList = discountList;



                return View(list);
            }
            catch (Exception ex) 
            {
                return View(list);
            }
        }



        public IActionResult Intro()
        {
            return View();
        }
        
    }
}
