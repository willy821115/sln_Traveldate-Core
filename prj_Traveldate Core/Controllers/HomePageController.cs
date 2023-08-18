using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult HomePage()
        {
            CHomePresent list = new CHomePresent();

            TraveldateContext db = new TraveldateContext();

            //var popular = from od in db.OrderDetails

            //        group od by od.Trip.Product into g

            //        orderby g.Count() descending

            //        select new { productId= g.Key.ProductId, productName= g.Key.ProductName, unitPrice = g.Min(od => od.Trip.UnitPrice) };
           
            //List<CHomeViewModel> popularList = new List<CHomeViewModel>();
            //foreach (var p in popular.Take(6).ToList())
            //{
            //    CHomeViewModel item = new CHomeViewModel();
            //    item.productId = p.productId;
            //    item.productName = p.productName;
            //    item.unitPrice = (decimal)p.unitPrice;

            //    var commentScore = db.CommentLists.Where(c => c.ProductId == p.productId).Select(c => c.CommentScore).Average();
            //    var imagePath = db.ProductPhotoLists.Where(c => c.ProductId == p.productId).Select(c => c.ImagePath).FirstOrDefault();

            //    item.ImagePath = imagePath;
            //    item.commentScore = commentScore;

            //    popularList.Add(item);
            //}

            //list.popularList = popularList;

            return View();
        }

        public IActionResult Intro()
        {
            return View();
        }
        
    }
}
