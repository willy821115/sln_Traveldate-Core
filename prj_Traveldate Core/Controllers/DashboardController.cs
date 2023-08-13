using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class DashboardController : Controller
    {
        private TraveldateContext _db = null;

        private int companyID=1;
        public DashboardController() 
        {
        _db= new TraveldateContext();
            //HttpContext.Session.SetInt32(CDictionary.SK_COMPANYID, 1);
           //companyID=  (int)HttpContext.Session.GetInt32(CDictionary.SK_COMPANYID);
        }
        public IActionResult List()
        {
            var orderdetails = from o in _db.OrderDetails
                               where o.Trip.Product.Company.CompanyId == companyID
                               select new { productType=o.Trip.Product.ProductType.ProductType, date = o.Trip.Date,TripID=o.TripId,Phone = o.Order.Member.Phone, productname = o.Trip.Product.ProductName, max = o.Trip.MaxNum };

          
            COrderState cOrderState = new COrderState();
            foreach (var order in orderdetails) 
            {
                cOrderState.ProductDetail = new List<CProductDetailViewModel>();
                CProductDetailViewModel cProductDetailViewModel = new CProductDetailViewModel();
                cProductDetailViewModel.productDate = order.date.Value.Date.ToString();
                cProductDetailViewModel.Phone = order.Phone;
                cProductDetailViewModel.productType = order.productType;
                cProductDetailViewModel.productName = order.productname;
                cProductDetailViewModel.stock = cProductDetailViewModel.TripStock((int)order.TripID);
                cOrderState.ProductDetail.Add(cProductDetailViewModel);
            }

            cOrderState.orderQuantity = cOrderState.OrderCount(companyID);
            cOrderState.turnover = cOrderState.Turnover(companyID);
            cOrderState.top3product = cOrderState.Top3(companyID);
            
            return View(cOrderState);
        }
        
    }
}
