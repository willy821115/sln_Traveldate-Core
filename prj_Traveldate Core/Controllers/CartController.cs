using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class CartController : Controller
    {
        int _memberID = 1;
        TraveldateContext _context;
        public CartController()
        {
            _context = new TraveldateContext();
        }
        public ActionResult ShoppingCart()
        {
            CShoppingCartViewModel vm = new CShoppingCartViewModel();
            vm.cartitems = new List<CCartItem>();
            vm.cartitems = _context.OrderDetails.Where(c => (c.Order.IsCart == true) && (c.Order.MemberId == _memberID)).Select(c=>
            new CCartItem
            {
                planName = c.Trip.Product.PlanName,
                date = $"{c.Trip.Date:d}",
                quantity = c.Quantity,
                photo = c.Trip.Product.ProductPhotoLists.FirstOrDefault().Photo,
                unitPrice = c.Trip.UnitPrice
        }).ToList();
            vm.recommends = _context.ProductLists.Take(4).ToList();
            return View(vm);
        }
        [HttpPost]
        public ActionResult ShoppingCart(int id)
        {
            return RedirectToAction("ConfirmOrder");
        }

        public ActionResult ConfirmOrder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ConfirmOrder(int id)
        {
            return RedirectToAction("CompleteOrder");
        }
        public ActionResult CompleteOrder()
        {
            return View();
        }
    }
}
