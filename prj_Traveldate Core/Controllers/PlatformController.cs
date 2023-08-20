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
            if (string.IsNullOrEmpty(vm.txtKeyword)) {
                var datas = from p in db.ProductLists
                            select new
                            {
                                company = p.CompanyId,
                                productName = p.ProductName,
                                productType = p.ProductType.ProductType,
                                city = p.City.City,
                                status = p.Status.Status1,
                            };
                foreach (var item in datas)
                {
                    CProductWrap cProductWrap = new CProductWrap();
                    cProductWrap.CompanyId = item.company;
                    cProductWrap.ProductName = item.productName;
                    cProductWrap.productType = item.productType;
                    cProductWrap.cityName = item.city;
                    cProductWrap.productStatus = item.status;
                    vm.product.Add(cProductWrap);
                }
            }

            else {
                var query = db.ProductLists
            .Where(p => p.ProductName.ToUpper().Contains(vm.txtKeyword.ToUpper()));

                if (vm.companySelect != "all")
                {
                    query = query.Where(p => p.Company.CompanyName == vm.companySelect);
                }

                if (vm.productTypeSelect != "all")
                {
                    query = query.Where(p => p.ProductType.ProductType == vm.productTypeSelect);
                }

                if (vm.statusSelect != "all")
                {
                    query = query.Where(p => p.Status.Status1 == vm.statusSelect);
                }

                vm.product = query
                    .Select(p => new CProductWrap
                    {
                        CompanyId = p.CompanyId,
                        ProductName = p.ProductName,
                        productType = p.ProductType.ProductType,
                        cityName = p.City.City,
                        productStatus = p.Status.Status1
                    })
                    .ToList();
            }
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
