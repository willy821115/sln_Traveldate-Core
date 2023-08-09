using Microsoft.AspNetCore.Mvc;

namespace prj_Traveldate_Core.Controllers
{
    public class CartController : Controller
    {
        public ActionResult ShoppingCart()
        {
            return View();
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
