using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;

namespace prj_Traveldate_Core.Controllers
{
    public class PlatformController : Controller
    {
       public IActionResult Review(CPlatformViewModel vm)
        {
            TraveldateContext db = new TraveldateContext();
            CProductFactory factory = new CProductFactory();
            vm.status = factory.loadStauts();
            vm.types = factory.loadTypes();
            vm.company = factory.loadCompanies();

            vm.product = new List<CProductWrap>();
            var datas = from p in db.ProductLists
                        select new
                        {
                            productName = p.ProductName,
                            productType = p.ProductType.ProductType,
                            city = p.City.City,
                            status = p.Status.Status1,
                        };
            foreach (var item in datas)
            {
                CProductWrap cProductWrap = new CProductWrap();
                cProductWrap.ProductName = item.productName;
                cProductWrap.productType = item.productType;
                cProductWrap.cityName = item.city;
                cProductWrap.productStatus = item.status;
                vm.product.Add(cProductWrap);
            }
            //IEnumerable<ProductList> datas = null;
            //if (string.IsNullOrEmpty(vm.txtKeyword))
            //    datas = from p in db.ProductLists select p;
            //else
            //    datas = db.ProductLists.Where(p=>p.ProductName.ToUpper().Contains(vm.txtKeyword.ToUpper()));
            return View(vm);
        }




        public ActionResult content1()
        {
            return View();
        }
        public ActionResult content2()
        {
            return View();
        }
        public ActionResult content3()
        {
            return View();
        }

        public ActionResult CouponCreate()
        {
            return View();
        }
    }
}
