using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult HomePage()
        {
            //CHomePresent list = new CHomePresent();

            //TraveldateContext db = new TraveldateContext();

            //var product = from p in  db.ProductLists
            //              join t in db.Trips on p.ProductId equals t.ProductId
            //              join o in db.OrderDetails on t.TripId equals o.TripId
            //              join c in db.CommentLists on p.ProductId equals c.ProductId
            //              group p by p.ProductId into g
                          


            ////var q = from p in db.Trips
            ////        orderby p.OrderDetails.Count descending
            ////        select new { productId = p.Product.ProductId,productName=p.Product.ProductName, unitPrice = p.UnitPrice, commentScore = p.Product.CommentLists.Select(t => t.CommentScore).FirstOrDefault(), ImagePath = p.Product.ProductPhotoLists.Select(t => t.ImagePath).FirstOrDefault() };
            //List<CHomeViewModel> popularList = new List<CHomeViewModel>();
            //foreach (var p in q)
            //{
            //    CHomeViewModel item = new CHomeViewModel();
            //    item.productId = p.productId;
            //    item.productName = p.productName;
            //    item.ImagePath = p.ImagePath;
            //    item.unitPrice = (decimal)p.unitPrice;
            //    item.commentScore = (int)p.commentScoreAverage;

            //    popularList.Add(item);
            //}

            //list.commentList = popularList;

            return View();
        }

        public IActionResult Intro()
        {
            return View();
        }
    }
}
