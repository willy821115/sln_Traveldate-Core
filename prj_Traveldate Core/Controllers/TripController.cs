using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class TripController : Controller
    {
       
        private int companyID = 1;

        public TripController() 
        {
        
            //HttpContext.Session.SetInt32(CDictionary.SK_COMPANYID, 1);
            //companyID=  (int)HttpContext.Session.GetInt32(CDictionary.SK_COMPANYID);
        }



        public IActionResult List()
        {
            TraveldateContext _db = new TraveldateContext();
            var products = from p in _db.ProductLists
                           where p.CompanyId == companyID
                           select new {productID=p.ProductId, productname = p.ProductName, productType = p.ProductType.ProductType, productStatus = p.Status.Status1 };
            //var trips = from t in _db.Trips
            //            where t.Product.CompanyId == companyID
            //            select new {tripID=t.TripId, productID=t.Product.ProductId, tripName=t.Product.ProductName, tripType =t.Product.ProductType.ProductType,tripDate=t.Date};

            CTripViewModel list = new CTripViewModel();
            list.cProductWraps = new List<CProductWrap>();
            list.cTripWraps = new List<CTripWrap>();

            foreach (var p in products) 
            {
            CProductWrap pro = new CProductWrap();
                pro.ProductName = p.productname;
                pro.productType = p.productType;
                pro.productStatus = p.productStatus;
                pro.ProductId = p.productID;
                list.cProductWraps.Add(pro);
            }
            CProductFactory cProductFactory = new CProductFactory();
            //            foreach (var t in trips) 
            //            {
            //            CTripWrap tr = new CTripWrap();

            //                tr.productName = t.tripName;
            //                tr.date= t.tripDate.Value.Date.ToString();
            //                tr.productType = t.tripType;
            //                tr.day = cProductFactory.TripDays(t.productID);
            //                tr.stock = cProductFactory.TripStock(t.tripID);
            //                list.cTripWraps.Add(tr);
            //             }
            list.types = cProductFactory.loadTypes();
            return View(list);
        }

        public IActionResult LoadTripTable(int productID) 
        {
            TraveldateContext db = new TraveldateContext();
CProductFactory cProductFactory = new CProductFactory();
            var trips = from t in db.Trips
                        where t.Product.ProductId == productID
                        select new { tripID = t.TripId,  tripName = t.Product.ProductName, tripType = t.Product.ProductType.ProductType, tripDate = t.Date,tripDay = cProductFactory.TripDays(t.ProductId), stock= cProductFactory.TripStock(t.TripId) };
       
            return Json(trips);
        }
        public IActionResult Create(int ProductId)
        {
            CTripWrap t = new CTripWrap();
            t.ProductId = ProductId;
            return View(t);
        }
        [HttpPost]
        public IActionResult Create(CTripWrap trip)
        {
            TraveldateContext db = new TraveldateContext();
            if (trip.tripDates != null) 
            {
            string[] dates= trip.tripDates.Replace(" ", "").Split(",");
                foreach (string date in dates) 
                {
                    Trip t = new Trip();
                    t.Date = DateTime.Parse(date);
                    t.ProductId=trip.ProductId;
                    t.UnitPrice = trip.UnitPrice;
                    t.MinNum = trip.MinNum;
                    t.MaxNum = trip.MaxNum;
                    if (trip.Discount!=null)
                    {
                        t.Discount = trip.Discount;
                        t.DiscountExpirationDate = DateTime.Parse(trip.discountLimitDate);
                    }
                    db.Trips.Add(t);
                 }
                db.SaveChanges();
            }
           
            return RedirectToAction("List");
        }

        public IActionResult Edit() 
        { 
            return View(); 
        }
    }
}
