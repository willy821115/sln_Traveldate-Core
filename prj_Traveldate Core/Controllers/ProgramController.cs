using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;
using System;
using System.Globalization;

namespace prj_Traveldate_Core.Controllers
{
    public class ProgramController : Controller
    {
        public IActionResult Product(int? id)
        {
            if (id == null)
                return RedirectToAction("SearchList", "Search");
            CProductFactory pf = new CProductFactory();
            ProgramViewModel vm = new ProgramViewModel();
            vm.program.fPhotoList = pf.loadPhoto((int)id);
            vm.product.ProductName = pf.loadTitle((int)id);
            vm.product.Description = pf.loadDescription((int)id);
            vm.program.fTripDate = pf.loadTrip((int)id);
            vm.product.PlanName = pf.loadPlan((int)id);
            vm.program.fPlanDescription = pf.LoadPlanDescri((int)id);
            vm.product.Address= pf.loadAddress((int)id);
            vm.program.fOutline = pf.loadOutlineDetails((int)id);
            vm.city.City = pf.loadCity((int)id);
            vm.program.fComMem=pf.loadCommentMem((int)id);
            vm.program.fPlanPrice = pf.loadPlanprice((int)id);
            vm.program.fTripDetails = pf.loadTripdetails((int)id);
            vm.program.fTripPrice = pf.loadPlanpriceStart((int)id);
            vm.program.fComMemGender= pf.memgender((int)id);
            vm.program.fCommentDate = pf.loadCommentDate((int)id);
            vm.program.fComScore = pf.loadcommentScore((int)id);
            vm.program.fComTitle = pf.loadCommentTitle((int)id);
            vm.program.fComContent = pf.loadCommentContent((int)id);
            vm.program.fStatus = pf.loadStatus((int)id);
            vm.product.ProductId = (int)id;
            return View(vm);
        }
        

        public IActionResult Address(int id)
        {
            TraveldateContext db = new TraveldateContext();
            var address = db.ProductLists.Where(p => p.ProductId == id).Select(p => p.Address).ToList();
            return Json(address);
        }

        public IActionResult MaxNum(string selectedDate,int id)
        {
            TraveldateContext db = new TraveldateContext();

            List<DateTime?> tripdate = db.Trips.Where(p=>p.ProductId==id).Select(t => t.Date).ToList();
            List<int?> max = db.Trips.Where(p => p.ProductId == id).Select(t => t.MaxNum).ToList();
            List<int?> min = db.Trips.Where(p => p.ProductId == id).Select(t => t.MinNum).ToList();
            List<decimal?> price = db.Trips.Where(p => p.ProductId == id).Select(t => t.UnitPrice).ToList();
            List<int> tripid = db.Trips.Where(p => p.ProductId == id).Select(t => t.TripId).ToList();

            int? maxnum = max[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            int? minnum = min[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            decimal? pricenum = price[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            int tripId = tripid[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            DateTime? date = tripdate.FirstOrDefault(d => d?.ToString("yyyy-MM-dd") == selectedDate);
            return Json(new
            {
                Maxnum = maxnum,
                Minnum = minnum,
                Price = pricenum,
                Date = date?.ToString("yyyy-MM-dd"),
                TripId = tripId
            });
        }


        
        [HttpPost]
        public IActionResult AddToCart(int num, int tripId)
        {
            TraveldateContext db = new TraveldateContext();

            int loggedInMemberId =1;
            int productId = 1;

            var existCart = db.Orders.Any(o => o.MemberId == loggedInMemberId && o.IsCart == true);
            if (existCart)
            {

                if (db.OrderDetails.Any(o => o.TripId == tripId && o.Order.MemberId == loggedInMemberId && o.Order.IsCart == true))
                {
                    OrderDetail od = db.OrderDetails.FirstOrDefault(o => o.TripId == tripId && o.Order.MemberId == loggedInMemberId && o.Order.IsCart == true);
                    od.Quantity += num;
                }
                else
                {
                    int cartOrderID = db.Orders.FirstOrDefault(o => o.MemberId == loggedInMemberId && o.IsCart == true).OrderId;
                    OrderDetail newOrderDetail = new OrderDetail
                    {
                        Quantity = num,
                        TripId = tripId,
                        OrderId = cartOrderID
                    };
                    db.OrderDetails.Add(newOrderDetail);
                }
                db.SaveChanges();
            }
            else
            {
                Order newCartOrder = new Order
                {
                    MemberId = loggedInMemberId,
                    IsCart = true
                };
                db.Orders.Add(newCartOrder);
                db.SaveChanges();
            }

            return RedirectToAction("Product");
        }

        [HttpGet]
        public IActionResult GetTripID(string selectedDate, int productId)
        {
            TraveldateContext db = new TraveldateContext();

            DateTime date = DateTime.ParseExact(selectedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            int tripID = db.Trips
                .Where(t => t.ProductId == productId && t.Date == date)
                .Select(t => t.TripId)
                .FirstOrDefault();

            return Json(tripID);
        }

    }
}
