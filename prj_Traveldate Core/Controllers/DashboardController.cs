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

        private int companyID=0;
        public DashboardController() 
        {
        _db= new TraveldateContext();
            //HttpContext.Session.SetInt32(CDictionary.SK_COMPANYID, 1);
          
        }
        public IActionResult List()
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            var orderdetails = from o in _db.OrderDetails
                               where o.Trip.Product.Company.CompanyId == companyID
                               select new { productType=o.Trip.Product.ProductType.ProductType, date = o.Trip.Date,TripID=o.TripId, Email = o.Order.Member.Email, productname = o.Trip.Product.ProductName, max = o.Trip.MaxNum };

          
            COrderState cOrderState = new COrderState();
            cOrderState.ProductDetail = new List<CProductDetailViewModel>();
            foreach (var order in orderdetails) 
            {
                 CProductDetailViewModel cProductDetailViewModel = new CProductDetailViewModel();
                CProductFactory cProductFactory = new CProductFactory();
                cProductDetailViewModel.productDate = string.Format("{0:yyyy-MM-dd}", order.date) ;
                cProductDetailViewModel.Email = order.Email;
                cProductDetailViewModel.productType = order.productType;
                cProductDetailViewModel.productName = order.productname;
                cProductDetailViewModel.stock = cProductFactory.TripStock((int)order.TripID);
                cOrderState.ProductDetail.Add(cProductDetailViewModel);
            }
            CProductFactory factory = new CProductFactory();
            cOrderState.status = factory.loadStauts();
            cOrderState.types = factory.loadTypes();
            cOrderState.orderQuantity = cOrderState.OrderCount(companyID);
            cOrderState.turnover =string.Format("{0:0}",cOrderState.Turnover(companyID)) ;
            cOrderState.top3product = cOrderState.Top3(companyID);
             
            
            return View(cOrderState);
        }
        public IActionResult queryByType(int typeID)
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
             var orderdetails = from o in _db.OrderDetails
                               where o.Trip.Product.ProductTypeId==typeID&&o.Trip.Product.CompanyId == companyID
                               select new { productType = o.Trip.Product.ProductType.ProductType, date = o.Trip.Date, TripID = o.TripId, Email = o.Order.Member.Email, productname = o.Trip.Product.ProductName, max = o.Trip.MaxNum };
            
            List< CProductDetailViewModel > queryList= new List<CProductDetailViewModel>();
            foreach (var order in orderdetails)
            {
                CProductDetailViewModel cProductDetailViewModel = new CProductDetailViewModel();
                CProductFactory cProductFactory = new CProductFactory();
                cProductDetailViewModel.productDate = string.Format("{0:yyyy-MM-dd}", order.date);
                cProductDetailViewModel.Email = order.Email;
                cProductDetailViewModel.productType = order.productType;
                cProductDetailViewModel.productName = order.productname;
                cProductDetailViewModel.stock = cProductFactory.TripStock((int)order.TripID);
                queryList.Add(cProductDetailViewModel);
            }
            return Json(queryList);
        }

        public IActionResult queryByDate(string dates) 
         {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            string[] querydate = dates.Replace(" ","").Split("to");
            var orderdetails = from o in _db.OrderDetails
                               where o.Trip.Date > DateTime.Parse(querydate[0])&& o.Trip.Date < DateTime.Parse(querydate[1]) && o.Trip.Product.CompanyId == companyID
                               select new { productType = o.Trip.Product.ProductType.ProductType, date = o.Trip.Date, TripID = o.TripId, Email = o.Order.Member.Email, productname = o.Trip.Product.ProductName, max = o.Trip.MaxNum };

            List<CProductDetailViewModel> queryList = new List<CProductDetailViewModel>();
            foreach (var order in orderdetails)
            {
                CProductDetailViewModel cProductDetailViewModel = new CProductDetailViewModel();
                CProductFactory cProductFactory = new CProductFactory();
                cProductDetailViewModel.productDate = string.Format("{0:yyyy-MM-dd}", order.date);
                cProductDetailViewModel.Email = order.Email;
                cProductDetailViewModel.productType = order.productType;
                cProductDetailViewModel.productName = order.productname;
                cProductDetailViewModel.stock = cProductFactory.TripStock((int)order.TripID);
                queryList.Add(cProductDetailViewModel);
            }

            return Json(queryList);
        }


    }
}
