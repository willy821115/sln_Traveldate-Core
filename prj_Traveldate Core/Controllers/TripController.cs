using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class TripController : CompanySuperController
    {
       
        private int companyID = 0;

        public TripController() 
        {
        
            //HttpContext.Session.SetInt32(CDictionary.SK_COMPANYID, 1);
            //companyID=  (int)HttpContext.Session.GetInt32(CDictionary.SK_COMPANYID);
        }



        public IActionResult List()
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
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
            list.statuses = cProductFactory.loadStauts();
            return View(list);
        }

        public IActionResult LoadTripTable(int productID) 
        {
            TraveldateContext db = new TraveldateContext();
CProductFactory cProductFactory = new CProductFactory();
            var trips = from t in db.Trips
                        where t.Product.ProductId == productID
                        select new { tripID = t.TripId,  tripName = t.Product.ProductName, tripType = t.Product.ProductType.ProductType, tripDate = string.Format("{0:yyyy-MM-dd}", t.Date) ,tripDay =  cProductFactory.TripDays(t.ProductId),tripUnitPrice=t.UnitPrice ,stock= cProductFactory.TripStock(t.TripId) };
       
            return Json(trips);
        }
        public IActionResult Create(int id)
        {
            CTripWrap t = new CTripWrap();
            t.ProductId = id;
            return View(t);
        }
        [HttpPost]
        public IActionResult Create(CTripWrap trip)
        {
            Thread.Sleep(1000);
            TraveldateContext db = new TraveldateContext();
            try 
            {
            
          
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
            catch(Exception ex) 
            {
                return RedirectToAction("List");
            }
        }

        public IActionResult Edit(int tripID) 
        {
            TraveldateContext db = new TraveldateContext();
            var trips = db.Trips.FirstOrDefault(t => t.TripId == tripID);
            CTripWrap tr = new CTripWrap();
            tr.tripDates = string.Format("{0:yyyy-MM-dd}", trips.Date);
            tr.discountLimitDate = string.Format("{0:yyyy-MM-dd}", trips.DiscountExpirationDate);
            tr.MaxNum = trips.MaxNum;
            tr.MinNum = trips.MinNum;
            tr.UnitPrice = trips.UnitPrice;
            tr.Discount = trips.Discount;
            tr.TripId = tripID;
            return View(tr); 
        }
        [HttpPost]
        public IActionResult Edit(CTripWrap t) 
        {
            Thread.Sleep(1000);
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            TraveldateContext db = new TraveldateContext();
            try 
            {
            
            var tripEdit = db.Trips.FirstOrDefault(tr => tr.TripId == t.TripId);
            tripEdit.UnitPrice= t.UnitPrice;
            tripEdit.Discount = t.Discount;
                if(t.discountLimitDate!=null)
            tripEdit.DiscountExpirationDate = DateTime.Parse(t.discountLimitDate);
            tripEdit.MaxNum = t.MaxNum;
            tripEdit.MinNum = t.MinNum;
                if(t.tripDates!=null)
            tripEdit.Date = DateTime.Parse(t.tripDates);
            db.SaveChanges();
            return RedirectToAction("List");
            }
            catch (Exception ex) 
            {
                return RedirectToAction("List");
            }
        }

        public IActionResult queryByType(int typeID) 
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            TraveldateContext db = new TraveldateContext();
            CProductFactory pro = new CProductFactory();
            var q = db.ProductLists.Where(p => p.ProductTypeId == typeID && p.CompanyId == companyID).Select(p => new
            {
                productName = p.ProductName,
                productType = p.ProductType.ProductType,
                productStatus = p.Status.Status1,
                productID = p.ProductId

            });


            return Json(q);
        }

        public IActionResult queryByStatus(int statusID)
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            TraveldateContext db = new TraveldateContext();
            CProductFactory pro = new CProductFactory();
            var q = db.ProductLists.Where(p => p.StatusId == statusID && p.CompanyId == companyID).Select(p => new
            {
                productName = p.ProductName,
                productType = p.ProductType.ProductType,
                productStatus = p.Status.Status1,
                productID = p.ProductId
            });
            return Json(q);
        }

        public IActionResult queryByDate(string dates)
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            string[] querydate = dates.Replace(" ", "").Split("to");

            TraveldateContext db = new TraveldateContext();
            CProductFactory cProductFactory = new CProductFactory();
            var trips = from t in db.Trips
                        where t.Date > DateTime.Parse(querydate[0]) && t.Date < DateTime.Parse(querydate[1]) && t.Product.CompanyId == companyID
                        select new { tripID = t.TripId, tripName = t.Product.ProductName, tripType = t.Product.ProductType.ProductType, tripDate = string.Format("{0:yyyy-MM-dd}", t.Date), tripDay = cProductFactory.TripDays(t.ProductId), stock = cProductFactory.TripStock(t.TripId) };


            return Json(trips);
        }
    }
}
