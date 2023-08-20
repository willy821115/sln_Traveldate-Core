using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Controllers
{
    public class ProductController : Controller
    {
        private TraveldateContext _db = null;
        private int companyID = 1;
        private IWebHostEnvironment _enviro = null;//要找到照片串流的路徑需要IWebHostEnvironment
        public ProductController(IWebHostEnvironment p) //利用建構子將p注入全域的_enviro來使用，因為interface無法被new
        {
            _enviro = p;
        }
        //public ProductController() 
        //{
        //    _db = new TraveldateContext();
        //    HttpContext.Session.SetInt32(CDictionary.SK_COMPANYID, 1);
        //    companyID = (int)HttpContext.Session.GetInt32(CDictionary.SK_COMPANYID);
        //}
        public IActionResult List()
        {
            TraveldateContext _db = new TraveldateContext();
                var q = from p in _db.ProductLists
                    where p.CompanyId == companyID
                    select new {productID=p.ProductId, productName = p.ProductName, productType = p.ProductType.ProductType, city = p.City.City, status = p.Status.Status1, discontinued=p.Discontinued };
           CProductListViewModel model = new CProductListViewModel();
            CProductFactory factory = new CProductFactory();
            model.status = factory.loadStauts();
            model.types = factory.loadTypes();
            model.list = new List<CProductWrap>();
            foreach (var item in q)
            {
                CProductWrap cProductWrap = new CProductWrap();
                cProductWrap.ProductId = item.productID;
                cProductWrap.ProductName= item.productName;
                cProductWrap.productType = item.productType;
                cProductWrap.cityName = item.city;
                cProductWrap.productStatus = item.status;
                cProductWrap.Discontinued = item.discontinued;
                model.list.Add(cProductWrap);
            }
            
            return View(model);
        }
        public IActionResult Create()
        {
            CProductWrap list = new CProductWrap();
            CProductFactory factory = new CProductFactory();
            list.categoryAndTags = factory.loadCategories();
            list.countries = factory.loadCountries();
            list.cities = factory.loadCities();
            list.types = factory.loadTypes();
            list.CompanyId = companyID;
            return View(list);
        }
        [HttpPost]
        public IActionResult Create(CProductWrap pro)
         {
            int productID = 0;
            TraveldateContext db = new TraveldateContext();
            //存入ProductLists
            ProductList save = new ProductList();
            save.CompanyId = pro.CompanyId;
            save.ProductName= pro.ProductName;
            save.CityId= pro.CityId;
            save.Description = pro.Description;
            save.ProductTypeId = pro.ProductTypeId;
            save.StatusId = 2;
            save.PlanName = pro.PlanName;
            save.PlanDescription = pro.PlanDescription;
            save.Discontinued = false;
            save.Outline= pro.Outline;
            save.OutlineForSearch   = pro.OutlineForSearch;
            save.Address = pro.Address;
            
            db.ProductLists.Add(save);
            db.SaveChanges();
            //獲取ProductID
            productID = db.ProductLists.Where(p => p.ProductName == pro.ProductName).Select(p => p.ProductId).FirstOrDefault();
            //存入ProductTagList
            if (pro.Tags != null)
            {
                foreach (int tag in pro.Tags)
                {
                    ProductTagList t = new ProductTagList();
                    t.ProductId = productID;
                    t.ProductTagDetailsId = tag;
                    db.ProductTagLists.Add(t);
                }
            }

            //存入ProductPhotoList            
            if (pro.photos != null) 
            {
                foreach (IFormFile photo in pro.photos) 
                {
                    ProductPhotoList photoList = new ProductPhotoList();
                    string photoName = Guid.NewGuid().ToString() + ".jpg";//用Guid產生一個系統上不會重複的代碼，重新命名圖片
                    photoList.ImagePath = photoName;
                    photoList.ProductId = productID;
                    photo.CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
                    db.ProductPhotoLists.Add(photoList);
                }
            }
            //存入TripDetail
            if (pro.TripDetails.Count() == pro.TripDays.Count() && pro.TripDays.Count() == pro.TripDetailphotos.Count()&&pro.TripDays!=null) 
            {
                for (int i = 0; i < pro.TripDays.Count(); i++) 
                {
                TripDetail t =new TripDetail();
                    t.TripDay = pro.TripDays[i];
                    t.TripDetail1 = pro.TripDetails[i];
                    t.ProductId = productID;
                    //照片
                      string photoName = Guid.NewGuid().ToString() + ".jpg";//用Guid產生一個系統上不會重複的代碼，重新命名圖片
                        t.ImagePath = photoName;
                    pro.TripDetailphotos[i].CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
                        db.TripDetails.Add(t);
                }
            }
            
            db.SaveChanges();

            return RedirectToAction("List");
        }



        public IActionResult Edit(int productID) 
        {
            Console.WriteLine(productID);
            TraveldateContext db = new TraveldateContext();
            CProductWrap list = new CProductWrap();
            CProductFactory factory = new CProductFactory();
            list.categoryAndTags = factory.loadCategories();
            list.countries = factory.loadCountries();
            list.cities = factory.loadCities();
            list.types = factory.loadTypes();
            list.ProductList = db.ProductLists.Where(p => p.ProductId == productID).FirstOrDefault();
            list.Tags = db.ProductTagLists.Where(t => t.ProductId == productID).Select(t => (int)t.ProductTagDetailsId).ToList();
            list.CtripDetail = db.TripDetails.Where(t => t.ProductId == productID).ToList();
            return View(list);
        }
    }
}
