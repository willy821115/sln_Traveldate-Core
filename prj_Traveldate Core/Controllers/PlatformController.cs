using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace prj_Traveldate_Core.Controllers
{
    public class PlatformController : Controller
    {
        private IWebHostEnvironment _enviro = null;//要找到照片串流的路徑需要IWebHostEnvironment
        public PlatformController(IWebHostEnvironment p) //利用建構子將p注入全域的_enviro來使用，因為interface無法被new
        {
            _enviro = p;
        }
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
                                productid = p.ProductId,
                                company = p.Company.CompanyName,
                                productName = p.ProductName,
                                productType = p.ProductType.ProductType,
                                city = p.City.City,
                                status = p.Status.Status1,
                            };
            foreach (var item in datas)
                {
                    CProductWrap cProductWrap = new CProductWrap();
                    cProductWrap.ProductId = item.productid;
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
                                 Discount = m.Discount,
                                 Enable = m.Enable,
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
                                  ServerDescription = c.ServerDescription,
                                  ComEnablel = c.Enable
                              };

            var combinedData = new CPlatformViewModel
            {
                Members = memberData.ToList(),
                Companies = companyData.ToList()
            };


            return View(combinedData);
        }

        public IActionResult Coupon(CPlatformViewModel vm)
        {
            TraveldateContext db = new TraveldateContext();
            CPlatformFactory pf = new CPlatformFactory();
             
            vm.coupon = new List<CCouponWrap>();

            var datas = from c in db.CouponLists
                        orderby c.CouponListId descending
                        select new 
                        {
                            CouponListId = c.CouponListId,
                            CouponName = c.CouponName,
                            Discount = c.Discount,
                            Description = c.Description,
                            DueDate = c.DueDate,
                            ImagePath = c.ImagePath
                        };
            foreach (var item in datas)
            {
                CCouponWrap CouponWrap = new CCouponWrap();
                CouponWrap.CouponListId = item.CouponListId;
                CouponWrap.CouponName = item.CouponName;
                CouponWrap.Discount = item.Discount;
                CouponWrap.Description = item.Description;
                CouponWrap.DueDate = item.DueDate;
                CouponWrap.ImagePath = item.ImagePath;
                int couponNum = pf.couponNum(item.CouponListId);
                int couponUsedNum = pf.couponUsedNum(item.CouponListId);

                // 将计算结果存入对象
                CouponWrap.CouponNum = couponNum;
                CouponWrap.CouponUsedNum = couponUsedNum;
                vm.coupon.Add(CouponWrap);

            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult CreateCoupon(CCouponWrap cou)
        {
            TraveldateContext db = new TraveldateContext();
            //存入CouponList
            CouponList save = new CouponList();
            save.CouponName = cou.CouponName;
            save.Discount = cou.Discount;
            save.Description = cou.Description;
            save.DueDate = cou.DueDate;
            if (cou.photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                save.ImagePath = photoName;
                cou.photo.CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
            }
            db.CouponLists.Add(save);
            db.SaveChanges();

            return RedirectToAction("Coupon");
        }

        [HttpPost]
        public IActionResult EditCoupon(CCouponWrap cou)
        {
            TraveldateContext db = new TraveldateContext();
            //存入CouponList
            CouponList save = db.CouponLists.FirstOrDefault(c => c.CouponListId == cou.CouponListId);
            if (save != null)
            {
                save.CouponName = cou.CouponName;
                save.Discount = cou.Discount;
                save.Description = cou.Description;
                save.DueDate = cou.DueDate;
                if (cou.photo != null)
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    save.ImagePath = photoName;
                    cou.photo.CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
                }
                db.SaveChanges();
            }
           

            return RedirectToAction("Coupon");
        }




        public IActionResult Send()
        {
            TraveldateContext db = new TraveldateContext();

            var viewModel = new CCouponSendViewModel
            {
               Coupons = db.CouponLists.ToList(),
                Members = db.Members.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Send(CCouponSendViewModel vm)
        {
            TraveldateContext db = new TraveldateContext();
                var selectedCoupon = db.CouponLists.FirstOrDefault(c => c.CouponListId == vm.SelectedCouponId);

                if (selectedCoupon != null)
                {
                    foreach (var memberId in vm.SelectedMemberIds)
                    {
                        var coupon = new Coupon
                        {
                            CouponListId = selectedCoupon.CouponListId,
                            MemberId = memberId
                        };
                        db.Coupons.Add(coupon);
                    }

                    db.SaveChanges();

                TempData["CouponSentMessage"] = "優惠券已成功發放";
                return RedirectToAction("Coupon");
                }
            return View(vm);
        }




        [HttpPost]
        public ActionResult DisableMember(int memberId, bool enable)
        {
            try
            {
                TraveldateContext db = new TraveldateContext();
                var member = db.Members.FirstOrDefault(m => m.MemberId == memberId);

                if (member != null)
                {
                    member.Enable = enable; 
                    db.SaveChanges();
                    return Content(enable.ToString());
                }
                return Content("沒有此會員");
            }
            catch (Exception ex)
            {
                return Content("");
            }
        }
        [HttpPost]
        public IActionResult BulkDisableMembers(List<int> memberIds, bool enable)
        {
            try
            {
                TraveldateContext db = new TraveldateContext();
                var membersToUpdate = db.Members.Where(m => memberIds.Contains(m.MemberId)).ToList();

                foreach (var member in membersToUpdate)
                {
                    member.Enable = enable; 
                }
                db.SaveChanges(); 
                var success = true;
                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetProductDetails(int productId)
        {
            TraveldateContext db = new TraveldateContext();
            var product = (from p in db.ProductLists where p.ProductId == productId select p)
                          .FirstOrDefault();

                var productDetails = new
                {
                    ProductName = product.ProductName,
                    city = product.CityId,
                    description = product.Description,
                    planName = product.PlanName,
                    plandescription = product.PlanDescription,
                    outline = product.Outline,
                    outlineforsearch = product.OutlineForSearch,
                    address = product.Address,
                };

                return Json(productDetails);
            
        }


        [HttpGet]
        public IActionResult CouponDetails(int couponId)
        {
            TraveldateContext db = new TraveldateContext();
            var couponDetails = db.CouponLists.FirstOrDefault(c => c.CouponListId == couponId);
            if (couponDetails == null)
            {
                return Content("no data"); 
            }

            return Json(new
            {
                couponName = couponDetails.CouponName,
                couponDiscount = couponDetails.Discount,
                couponDescription = couponDetails.Description,
                couponImage = couponDetails.ImagePath
            });
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
