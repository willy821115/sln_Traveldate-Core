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
                return RedirectToAction("List");
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

            return View(vm);
        }
        public IActionResult List()
        {
            TraveldateContext db = new TraveldateContext();
            var datas = from p in db.ProductLists
                        select p;
            return View(datas);
        }

        public IActionResult Address(int id)
        {
            TraveldateContext db = new TraveldateContext();
            var address = db.ProductLists.Where(p => p.ProductId == id).Select(p => p.Address).ToList();
            return Json(address);
        }

    }
}
