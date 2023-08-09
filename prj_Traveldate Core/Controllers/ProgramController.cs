﻿using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.ViewModels;

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
            vm.product.Outline = pf.loadOutline((int)id);
            vm.product.Description = pf.loadDescription((int)id);
            vm.program.fTripDate = pf.loadTrip((int)id);
            vm.product.PlanName = pf.loadPlan((int)id);
            vm.product.PlanDescription = pf.loadPlanDescri((int)id);

            return View(vm);
        }
        public IActionResult List()
        {
            TraveldateContext db = new TraveldateContext();
            var datas = from p in db.ProductLists
                        select p;
            return View(datas);
        }
    }
}
