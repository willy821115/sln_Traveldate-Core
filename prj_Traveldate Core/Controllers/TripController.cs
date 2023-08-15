using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class TripController : Controller
    {
        private TraveldateContext _db = null;
        private int companyID = 1;

        public TripController() 
        {
        _db=new TraveldateContext();
            //HttpContext.Session.SetInt32(CDictionary.SK_COMPANYID, 1);
            //companyID=  (int)HttpContext.Session.GetInt32(CDictionary.SK_COMPANYID);
        }



        public IActionResult List()
        {
            var products = from p in _db.ProductLists
                           where p.CompanyId == companyID
                           select new { productname = p.ProductName, productType = p.ProductType.ProductType, productStatus = p.Status.Status1 };
            var trips = from t in _db.Trips
                        where t.Product.CompanyId == companyID
                        select new {tripID=t.TripId, productID=t.Product.ProductId, tripName=t.Product.ProductName, tripType =t.Product.ProductType.ProductType,tripDate=t.Date};

            CTripViewModel list = new CTripViewModel();
            list.cProductWraps = new List<CProductWrap>();
            list.cTripWraps = new List<CTripWrap>();

            foreach (var p in products) 
            {
            CProductWrap pro = new CProductWrap();
                pro.ProductName = p.productname;
                pro.productType = p.productType;
                pro.productStatus = p.productStatus;
                list.cProductWraps.Add(pro);
            }
CProductFactory cProductFactory = new CProductFactory();
            foreach (var t in trips) 
            {
            CTripWrap tr = new CTripWrap();
                
                tr.productName = t.tripName;
                tr.date= t.tripDate.Value.Date.ToString();
                tr.productType = t.tripType;
                tr.day = cProductFactory.TripDays(t.productID);
                tr.stock = cProductFactory.TripStock(t.tripID);
                list.cTripWraps.Add(tr);
             }
            list.types = cProductFactory.loadTypes();
            return View(list);
        }
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit() 
        { 
            return View(); 
        }
    }
}
