using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace prj_Traveldate_Core.Controllers
{
    public class ProgramController : Controller
    {
        public IActionResult Product(int? id)
        {
            if (id == null)
                return RedirectToAction("Search");
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

        //public IActionResult Addtocart(int id)
        //{

        //}

        public IActionResult MaxNum(string selectedDate,int id)
        {
            TraveldateContext db = new TraveldateContext();

            List<DateTime?> tripdate = db.Trips.Where(p=>p.ProductId==id).Select(t => t.Date).ToList();
            List<int?> max = db.Trips.Where(p => p.ProductId == id).Select(t => t.MaxNum).ToList();
            List<int?> min = db.Trips.Where(p => p.ProductId == id).Select(t => t.MinNum).ToList();

            int? maxnum = max[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            int? minnum = min[tripdate.FindIndex(d => d?.ToString("yyyy-MM-dd") == selectedDate)];
            return Json(new
            {
                Maxnum = maxnum,
                Minnum = minnum,
            });
        }
    }
}
