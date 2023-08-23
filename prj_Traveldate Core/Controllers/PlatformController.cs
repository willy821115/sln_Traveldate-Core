﻿using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.ComponentModel.Design;
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
            save.ImagePath = cou.ImagePath;

            db.CouponLists.Add(save);
            db.SaveChanges();

            return RedirectToAction("Coupon");
        }

        public IActionResult Send(int id)
        {
            TraveldateContext db = new TraveldateContext();
            // 根據 CouponListId 取得優惠券資訊
            var coupon = db.CouponLists.FirstOrDefault(c => c.CouponListId == id);

            if (coupon == null)
            {
                // 優惠券不存在，處理異常情況
                return RedirectToAction("Coupon"); // 假設這是返回到優惠券列表的動作
            }

            // 取得所有會員，供選擇會員進行發放
            var members = db.Members.ToList();

            // 將優惠券和會員列表傳遞到 View
            var viewModel = new CCouponSendViewModel
            {
                Coupon = coupon,
                Members = members
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SendCoupon(int SelectedMemberId, int CouponListId)
        {
            TraveldateContext db = new TraveldateContext();
            var coupon = db.CouponLists.FirstOrDefault(c => c.CouponListId == CouponListId);
            var member = db.Members.FirstOrDefault(m => m.MemberId == SelectedMemberId);

            if (coupon == null || member == null)
            {
                // 優惠券或會員不存在，處理異常情況
                return RedirectToAction("Coupon"); // 假設這是返回到優惠券列表的動作
            }

            // 執行優惠券發放的邏輯，例如建立關聯表等

            return RedirectToAction("Coupon"); // 假設這是返回到優惠券列表的動作
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
