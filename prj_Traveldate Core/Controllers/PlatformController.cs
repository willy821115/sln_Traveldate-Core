using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                            orderby p.Status descending
                            select new
                            {
                                company = p.Company.CompanyName,
                                productName = p.ProductName,
                                productType = p.ProductType.ProductType,
                                city = p.City.City,
                                status = p.Status.Status1,
                            };
            foreach (var item in datas)
                {
                    CProductWrap cProductWrap = new CProductWrap();
                    cProductWrap.CompanyName = item.company;
                    cProductWrap.ProductName = item.productName;
                    cProductWrap.productType = item.productType;
                    cProductWrap.cityName = item.city;
                    cProductWrap.productStatus = item.status;
                    vm.product.Add(cProductWrap);
                }

            return View(vm);
        }

        public IActionResult AccountSuspend()
        {
            TraveldateContext db = new TraveldateContext();
            var memberData = from m in db.Members
                             select new CPlatformMemViewModel
                             {
                                 MemberId = m.MemberId,
                                 LastName = m.LastName,
                                 FirstName = m.FirstName,
                                 Gender = m.Gender,
                                 Idnumber = m.Idnumber,
                                 BirthDate = m.BirthDate,
                                 Phone = m.Phone,
                                 Email = m.Email,
                                 Discount = m.Discount
                             };

            var companyData = from c in db.Companies
                              select new CPlatformMemViewModel
                              {
                                  CompanyId = c.CompanyId,
                                  TaxIdNumber = c.TaxIdNumber,
                                  CompanyName = c.CompanyName,
                                  City = c.City,
                                  Address = c.Address,
                                  CompanyPhone = c.Phone,
                                  Principal = c.Principal,
                                  Contact = c.Contact,
                                  Title = c.Title,
                                  ComEmail = c.Email,
                                  ServerDescription = c.ServerDescription
                              };

            var combinedData = new CPlatformViewModel
            {
                Members = memberData.ToList(),
                Companies = companyData.ToList()
            };


            return View(combinedData);
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
