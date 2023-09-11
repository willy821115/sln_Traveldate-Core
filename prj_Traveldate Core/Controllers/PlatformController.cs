using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using RazorEngine;
using RazorEngine.Templating;
using System.ComponentModel.Design;
using System.Configuration;
using System.Drawing;
using System.Net.Mail;
using System.Net.Mime;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prj_Traveldate_Core.Controllers
{
    public class PlatformController : PlatformSuperController
    {
        private IWebHostEnvironment _enviro ;//要找到照片串流的路徑需要IWebHostEnvironment
        private readonly IConfiguration _configuration;
        public PlatformController(IWebHostEnvironment p, IConfiguration configuration) //利用建構子將p注入全域的_enviro來使用，因為interface無法被new
        {
            _enviro = p;
            _configuration = configuration;
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
                    memberIdsToSend = db.Members.Where(m=>m.Enable==true).Select(m => m.MemberId).ToList();
                }
                else
                {
                    if (vm.SendToNormalMembers)
                    {
                        var normalMemberIds = db.Members.Where(m => m.LevelId == 1 && m.Enable == true).Select(m => m.MemberId).ToList();
                        memberIdsToSend.AddRange(normalMemberIds);
                    }
                    if (vm.SendToSilverMembers)
                    {
                        var silverMemberIds = db.Members.Where(m => m.LevelId == 2 && m.Enable == true).Select(m => m.MemberId);
                        memberIdsToSend.AddRange(silverMemberIds);
                    }
                    if (vm.SendToGoldMembers)
                    {
                        var goldMemberIds = db.Members.Where(m => m.LevelId == 3 && m.Enable == true).Select(m => m.MemberId);
                        memberIdsToSend.AddRange(goldMemberIds);
                    }
                    if (vm.SendToDiamondMembers)
                    {
                        var diamondMemberIds = db.Members.Where(m => m.LevelId == 4 && m.Enable == true).Select(m => m.MemberId);
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

                string templatePath = "Views/Emails/CouponEmailTemplate.cshtml";
                CCouponEmailViewModel mail = new CCouponEmailViewModel();
               
                mail.title = "專屬優惠降臨!!!";
                mail.content1 = "您的優惠券已歸戶，快到TravelDate找行程出遊去～";
                mail.CouponName = selectedCoupon.CouponName;
                mail.CouponDescription = selectedCoupon.Description;
                //顯示XX折
                decimal discountValue = (decimal)selectedCoupon.Discount * 100;

                var discountText = discountValue % 10 == 0
                    ? $"{discountValue / 10:F0} 折"
                    : $"{discountValue:F0} 折";

                mail.CouponDiscount = discountText;

                mail.CouponExpDate = System.String.Format("{0:yyyy-MM-dd}", selectedCoupon.DueDate);
                mail.buttonText = "立即使用優惠";
                mail.buttonLink = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.ToString() + "/";
                mail.contentFooter = "【使用說明】進入會員專區並點選優惠券清單，結帳時即可使用。";
               
                string imagePath = _enviro.WebRootPath +"/images/" + selectedCoupon.ImagePath;
                string catchimg = _enviro.WebRootPath + "/images/democoupon.jpg";
                try
                {
                    //建立連結資源
                    var res = new LinkedResource(imagePath);
                    res.ContentId = Guid.NewGuid().ToString();

                    //使用<img src="/img/loading.svg" data-src="cid:..."方式引用內嵌圖片
                    mail.CouponImg = $@"cid:{res.ContentId}";

                    var memname = db.Members.Select(m => m.FirstName).FirstOrDefault();
                    List<string> UserEmail = db.Members.Select(m => m.Email).ToList();
                    string sendmail = "weilunjiang3737@gmail.com";
                    string sentobowen = "bowenchang7979@gmail.com";
                    //string sendmail = "traveldate3@gmail.com";

                    LoginApiController api = new LoginApiController(_configuration, HttpContext);
                    if (UserEmail.Equals(sendmail))
                    {
                        mail.userName = "Hi, " + memname;
                    }
                    if (UserEmail.Equals(sentobowen))
                    {
                        mail.userName = "Hi, 柏文";
                    }
                    else
                    {
                        mail.userName = "Hi, " + memname;
                    }
                    string mailSubject = "專屬好禮來臨！快到Traveldate享用優惠～";

                    string mailContent = Engine.Razor.RunCompile(System.IO.File.ReadAllText(templatePath), "getcoupon", typeof(CCouponEmailViewModel), mail);

                    //建立AlternativeView
                    var altView = AlternateView.CreateAlternateViewFromString(
                        mailContent, null, MediaTypeNames.Text.Html);
                    //將圖檔資源加入AlternativeView
                    altView.LinkedResources.Add(res);
                    api.SimplySendMail(mailSubject, UserEmail, altView);
                }
                catch (IOException)
                {
                    //建立連結資源
                    var res = new LinkedResource(catchimg);
                    res.ContentId = Guid.NewGuid().ToString();

                    //使用<img src="/img/loading.svg" data-src="cid:..."方式引用內嵌圖片
                    mail.CouponImg = $@"cid:{res.ContentId}";

                    var memname = db.Members.Select(m => m.FirstName).FirstOrDefault();
                    List<string> UserEmail = db.Members.Select(m => m.Email).ToList();
                     string sendmail = "weilunjiang3737@gmail.com";
                    string sentobowen = "bowenchang7979@gmail.com";
                    //string sendmail = "traveldate3@gmail.com";

                    LoginApiController api = new LoginApiController(_configuration, HttpContext);
                    if (UserEmail.Equals(sendmail))
                    {
                        mail.userName = "Hi, " + memname;
                    }
                   if (UserEmail.Equals(sentobowen))
                    {
                        mail.userName = "Hi, 柏文";
                    }
                    else
                    {
                        mail.userName = "Hi, " + memname;
                    }
                    string mailSubject = "專屬好禮來臨！快到Traveldate享用優惠～";

                    string mailContent = Engine.Razor.RunCompile(System.IO.File.ReadAllText(templatePath), "getcoupon", typeof(CCouponEmailViewModel), mail);

                    //建立AlternativeView
                    var altView = AlternateView.CreateAlternateViewFromString(
                        mailContent, null, MediaTypeNames.Text.Html);
                    //將圖檔資源加入AlternativeView
                    altView.LinkedResources.Add(res);
                    api.SimplySendMail(mailSubject, UserEmail, altView);
                }

             

                TempData["CouponSentMessage"] = "優惠券 & 優惠電子報 已成功發放";
                
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
                var company = db.Companies.FirstOrDefault(c => c.CompanyId == product.CompanyId);
                var protype = db.ProductTypeLists.FirstOrDefault(c => c.ProductTypeId == product.ProductTypeId);
                var protags = db.ProductTagLists.Where(c => c.ProductId == product.ProductId).Select(p => p.ProductTagDetails.ProductTagDetailsName).ToList(); ;
               
                var tripdetail = db.TripDetails.Where(t=>t.ProductId == product.ProductId ).Select(t=>t.TripDetail1).ToList();
                var propic = db.ProductPhotoLists.Where(p => p.ProductId == product.ProductId).Select(p => p.ImagePath).ToList();

                if (city != null)
                {
                    var productDetails = new
                    {
                        CompanyName = company.CompanyName,
                        ProductName = product.ProductName,
                        ProductType = protype.ProductType,
                        CityName = city.City,
                        Description = product.Description,
                        PlanName = product.PlanName,
                        PlanDescription = product.PlanDescription,
                        Outline = product.Outline,
                        OutlineForSearch = product.OutlineForSearch,
                        Address = product.Address,
                        Photos = propic,
                        Prodtags = protags,
                        PlanDesc = product.PlanDescription,
                        tripdetail = tripdetail,
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

            decimal discountValue = (decimal)couponDetails.Discount * 100;

            var discountText = discountValue % 10 == 0
                ? $"{discountValue / 10:F0} 折"
                : $"{discountValue:F0} 折";

            return Json(new
            {
                couponName = couponDetails.CouponName,
                couponDiscount = discountText,
                couponDescription = couponDetails.Description,
                couponDate = System.String.Format("{0:yyyy-MM-dd}", couponDetails.DueDate),
                couponImage = couponDetails.ImagePath
            });
        }


        public IActionResult SusMemAccount(int memberId)
        {
            TraveldateContext db = new TraveldateContext();
            CProductFactory pro = new CProductFactory(); 
            var q = db.Members.FirstOrDefault(m => m.MemberId == memberId);
            q.Enable = !q.Enable;
            db.SaveChanges();
            var q2 = db.Members.Where(m => m.MemberId == memberId).Select(m => new
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
                Enable = (bool)m.Enable ? "停權" : "復原"
            });
            return Json(q2.ToList());

        }


        public IActionResult SusComAccount(int comId)
        {
            TraveldateContext db = new TraveldateContext();
            CProductFactory pro = new CProductFactory();
            var q = db.Companies.FirstOrDefault(m => m.CompanyId == comId);
            q.Enable = !q.Enable;
            db.SaveChanges();
            var companyData = from c in db.Companies
                              select new
                              {
                                  companyId = c.CompanyId,
                                  taxIdNumber = c.TaxIdNumber,
                                  companyName = c.CompanyName,
                                  city = c.City,
                                  address = c.Address,
                                  companyPhone = c.Phone,
                                  contact = c.Contact,
                                  title = c.Title,
                                  comEmail = c.Email,
                                  serverDescription = c.ServerDescription,
                                  comEnable = (bool)c.Enable ? "正常狀態" : "已停權"
                              };
            
            return Json(companyData.ToList());
        }
       
        public IActionResult SusAllComAccount([FromBody] List<int> comIds)
        {
            TraveldateContext db = new TraveldateContext();
            CProductFactory pro = new CProductFactory();
            foreach (var comId in comIds)
            {
                var com = db.Companies.FirstOrDefault(c => c.CompanyId == comId);

                if (com != null)
                {
                    com.Enable = !com.Enable;
                }
            }
            db.SaveChanges();
            var companyData = from c in db.Companies
                              select new
                              {
                                  companyId = c.CompanyId,
                                  taxIdNumber = c.TaxIdNumber,
                                  companyName = c.CompanyName,
                                  city = c.City,
                                  address = c.Address,
                                  companyPhone = c.Phone,
                                  contact = c.Contact,
                                  title = c.Title,
                                  comEmail = c.Email,
                                  serverDescription = c.ServerDescription,
                                  comEnable = (bool)c.Enable ? "正常狀態" : "已停權"
                              };

            return Json(companyData.ToList());
        }

        public IActionResult queryByStatus(string statusId)
        {
            TraveldateContext db = new TraveldateContext();
            IQueryable<Company> companies;

            if (statusId == "allcom")
            {
                companies = db.Companies;
            }
            else if (statusId == "activecom")
            {
                companies = db.Companies.Where(c => c.Enable == true);
            }
            else if (statusId == "suspendcom")
            {
                companies = db.Companies.Where(c => (bool)c.Enable == false);
            }
            else
            {
                // 預設顯示所有供應商
                companies = db.Companies;
            }
            var companyData = companies.Select(c => new
            {
                companyId = c.CompanyId,
                taxIdNumber = c.TaxIdNumber,
                companyName = c.CompanyName,
                city = c.City,
                address = c.Address,
                companyPhone = c.Phone,
                contact = c.Contact,
                title = c.Title,
                comEmail = c.Email,
                serverDescription = c.ServerDescription,
                comEnable = (bool)c.Enable ? "正常狀態" : "已停權"
            });
            return Json(companyData);
        }



        
        public IActionResult ReviewProduct(int id)
        {
            TraveldateContext db = new TraveldateContext();
            var product = db.ProductLists.FirstOrDefault(p => p.ProductId == id);

            if (product != null)
            {
                var city = db.CityLists.FirstOrDefault(c => c.CityId == product.CityId);
                var company = db.Companies.FirstOrDefault(c => c.CompanyId == product.CompanyId);
                var protype = db.ProductTypeLists.FirstOrDefault(c => c.ProductTypeId == product.ProductTypeId);
                var protags = db.ProductTagLists.Where(c => c.ProductId == product.ProductId).Select(p => p.ProductTagDetails.ProductTagDetailsName).ToList(); ;

                var tripdate = db.Trips.Where(t=>t.ProductId == product.ProductId).OrderBy(t=>t.Date).Select(t=>t.Date).ToList();

                var formatTripDates = tripdate.Select(d => d?.ToString("yyyy-MM-dd")).ToList();
                var tripprice = db.Trips.Where(t=>t.ProductId == product.ProductId).OrderBy(t => t.Date).Select(t=>t.UnitPrice).ToList();
                var tripdiscount = db.Trips.Where(t=>t.ProductId == product.ProductId).OrderBy(t => t.Date).Select(t=>t.Discount).ToList();
                var tripdetail = db.TripDetails.Where(t => t.ProductId == product.ProductId).Select(t => t.TripDetail1).ToList();
                var propic = db.ProductPhotoLists.Where(p => p.ProductId == product.ProductId).Select(p => p.ImagePath).ToList();
                var trippic = db.TripDetails.Where(td => td.ProductId == id).OrderBy(t => t.TripDay).Select(t => t.ImagePath).ToList();

                if (city != null)
                {
                    var productDetails = new
                    {
                        ProductId = product.ProductId,
                        CompanyName = company.CompanyName,
                        ProductName = product.ProductName,
                        ProductType = protype.ProductType,
                        CityName = city.City,
                        Description = product.Description,
                        PlanName = product.PlanName,
                        PlanDescription = product.PlanDescription,
                        Outline = product.Outline,
                        OutlineForSearch = product.OutlineForSearch,
                        Address = product.Address,
                        Photos = propic,
                        Prodtags = protags,
                        tripdetail = tripdetail,
                        trippic = trippic,
                        tripdate = formatTripDates,
                        tripprice = tripprice,
                        tripdiscount = tripdiscount
                    };

                    return View(productDetails);
                }
            }
            return NotFound();
        }



        [HttpPost]
        public IActionResult ReviewAction(string action, int productId)
        {
            using (var db = new TraveldateContext())
            {
                var product = db.ProductLists.FirstOrDefault(p => p.ProductId == productId);

                if (product != null)
                {
                    if (action == "reject")
                    {
                        product.StatusId = 3; 
                    }
                    else if (action == "approve")
                    {
                        product.StatusId = 1; 
                    }

                    db.SaveChanges(); 
                    return RedirectToAction("Review");
                }
            }
            return RedirectToAction("Review");
        }
    }
}
