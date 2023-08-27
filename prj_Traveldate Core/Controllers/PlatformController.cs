using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.ComponentModel.Design;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace prj_Traveldate_Core.Controllers
{
    public class PlatformController : PlatformSuperController
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
                                  ComEnable = c.Enable
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
                            CreateDate = c.CreateDate,
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
                CouponWrap.CreateDate = item.CreateDate;
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
            save.CreateDate = cou.CreateDate;
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
                List<int> memberIdsToSend = new List<int>();

                if (vm.SendToAllMembers)
                {
                    memberIdsToSend = db.Members.Select(m => m.MemberId).ToList();
                }
                else
                {
                    if (vm.SendToNormalMembers)
                    {
                        var normalMemberIds = db.Members.Where(m => m.LevelId == 1).Select(m => m.MemberId).ToList();
                        memberIdsToSend.AddRange(normalMemberIds);
                    }
                    if (vm.SendToSilverMembers)
                    {
                        var silverMemberIds = db.Members.Where(m => m.LevelId == 2).Select(m => m.MemberId);
                        memberIdsToSend.AddRange(silverMemberIds);
                    }
                    if (vm.SendToGoldMembers)
                    {
                        var goldMemberIds = db.Members.Where(m => m.LevelId == 3).Select(m => m.MemberId);
                        memberIdsToSend.AddRange(goldMemberIds);
                    }
                    if (vm.SendToDiamondMembers)
                    {
                        var diamondMemberIds = db.Members.Where(m => m.LevelId == 4).Select(m => m.MemberId);
                        memberIdsToSend.AddRange(diamondMemberIds);
                    }
                }

                foreach (var memberId in memberIdsToSend)
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
        public ActionResult SusMember(int memberId)
        {
                TraveldateContext db = new TraveldateContext();
                var member = db.Members.FirstOrDefault(m => m.MemberId == memberId);

                if (member != null)
                {
                    if(member.Enable == true)
                    {
                        member.Enable = false;
                }
                    else if(member.Enable == false)
                        {
                            member.Enable = true;
                }
                db.SaveChanges();
                return RedirectToAction("AccountSuspend");
            }
                return Content("沒有此會員");
        }

        [HttpPost]
        public ActionResult SusAllMember(List<int> memberIds)
        {
            using (var db = new TraveldateContext())
            {
                foreach (var memberId in memberIds)
                {
                    var member = db.Members.FirstOrDefault(m => m.MemberId == memberId);

                    if (member != null)
                    {
                        member.Enable = !member.Enable; 
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("AccountSuspend");
        }

        [HttpPost]
        public ActionResult SusCompany(int comId)
        {
            TraveldateContext db = new TraveldateContext();
            var com= db.Companies.FirstOrDefault(c => c.CompanyId== comId);

            if (com != null)
            {
                if (com.Enable == true)
                {
                    com.Enable = false;
                }
                else if (com.Enable == false)
                {
                    com.Enable = true;
                }
                db.SaveChanges();
                return RedirectToAction("AccountSuspend");
            }
            return Content("沒有此供應商");
        }

        [HttpPost]
        public ActionResult SusAllCom(List<int> comIds)
        {
            using (var db = new TraveldateContext())
            {
                foreach (var comId in comIds)
                {
                    var com = db.Companies.FirstOrDefault(c => c.CompanyId == comId);

                    if (com != null)
                    {
                        com.Enable = !com.Enable;
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("AccountSuspend");
        }


        [HttpGet]
        public IActionResult GetProductDetails(int productId)
        {
            TraveldateContext db = new TraveldateContext();
            var product = db.ProductLists.FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
                var city = db.CityLists.FirstOrDefault(c => c.CityId == product.CityId);
                var protype = db.ProductTypeLists.FirstOrDefault(c => c.ProductTypeId == product.ProductTypeId);
                var tripdetail = db.TripDetails.Where(t=>t.ProductId == product.ProductId ).Select(t=>t.TripDetail1).ToList();

                if (city != null)
                {
                    var productDetails = new
                    {
                        ProductName = product.ProductName,
                        ProductType = protype.ProductType,
                        CityName = city.City,
                        Description = product.Description,
                        PlanName = product.PlanName,
                        PlanDescription = product.PlanDescription,
                        Outline = product.Outline,
                        OutlineForSearch = product.OutlineForSearch,
                        Address = product.Address,
                    };

                    return Json(productDetails);
                }
            }

            return NotFound();

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
                couponDate = String.Format("{0:yyyy-MM-dd}", couponDetails.DueDate),
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
