using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System.Collections.Generic;
using System.IO;

namespace prj_Traveldate_Core.Controllers
{
    public class ProductController : Controller
    {
        private TraveldateContext _db = null;
        private int companyID = 3;
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
            foreach (TripDetailText t in pro.triptest) 
            {
                TripDetail trip = new TripDetail();
                trip.TripDay = t.TripDay;
                trip.TripDetail1 = t.TripDetail;
                trip.ProductId = productID;
                //照片
                if (t.photo != null) 
                {
                 string photoName = Guid.NewGuid().ToString() + ".jpg";//用Guid產生一個系統上不會重複的代碼，重新命名圖片
                trip.ImagePath = photoName;
                t.photo.CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
                }
               db.TripDetails.Add(trip);
            }


            db.SaveChanges();

            return RedirectToAction("List");
        }



        public IActionResult Edit(int productID) 
        {
            TraveldateContext db = new TraveldateContext();
            CProductWrap list = new CProductWrap();
            CProductFactory factory = new CProductFactory();
            list.categoryAndTags = factory.loadCategories();
            list.countries = factory.loadCountries();
            list.cities = factory.loadCities();
            list.types = factory.loadTypes();
            list.ProductList = db.ProductLists.Where(p => p.ProductId == productID).FirstOrDefault();
            list.Tags = db.ProductTagLists.Where(t => t.ProductId == productID).Select(t => (int?)t.ProductTagDetailsId).ToList();
            list.CtripDetail = db.TripDetails.Where(t => t.ProductId == productID).ToList();
            return View(list);
        }

        [HttpPost]

        public IActionResult Edit(CProductWrap pro) 
        {
            TraveldateContext db = new TraveldateContext();
            ProductList proDb=db.ProductLists.FirstOrDefault(p=>p.ProductId==pro.ProductId);
           //存入ProductList
            if (proDb != null) 
            {
                proDb.ProductName = pro.ProductName;
                proDb.CityId = pro.CityId;
                proDb.Description = pro.Description;
                proDb.ProductTypeId = pro.ProductTypeId;
                proDb.PlanName = pro.PlanName;
                proDb.PlanDescription = pro.PlanDescription;
                proDb.Outline = pro.Outline;
                proDb.OutlineForSearch = pro.OutlineForSearch;
                proDb.Address = pro.Address;
            }
            //存入ProductTagList
            var originalList = db.ProductTagLists.Where(p => p.ProductId == pro.ProductId).Select(p => p.ProductTagDetailsId).ToList();
            var addedTagID = pro.Tags.Except(originalList).ToList();
            var deletedTagID = originalList.Except(pro.Tags).ToList();
            //刪除移除的Tag
            var tagDbDelete = db.ProductTagLists.Where(t => t.ProductId == pro.ProductId && deletedTagID.Contains(t.ProductTagDetailsId)).ToList();
                                       
            if (deletedTagID != null) 
            {
                foreach (var tag in tagDbDelete) 
                {
                db.ProductTagLists.Remove(tag);
                }
            }
            //新增新加的Tag
            if (addedTagID != null)
            {
                foreach (int tagID in addedTagID)
            {
            ProductTagList tagDbAdd=new ProductTagList();
                tagDbAdd.ProductTagDetailsId = tagID;
                tagDbAdd.ProductId = pro.ProductId;
                db.Add(tagDbAdd);
            }
                          
            }
            //存入ProductPhotoList            
            if (pro.photos != null)
            {
                //先刪除image所有輪播圖
                var deletedPhoto = db.ProductPhotoLists.Where(p => p.ProductId == pro.ProductId);
                
                if (deletedPhoto != null) 
                {
                    foreach (var photo in deletedPhoto)
                    {
                        db.ProductPhotoLists.Remove(photo);
                    }
                    foreach (var path in deletedPhoto.Select(t=>t.ImagePath)) 
                    {
                        if (path != null) 
                        {
                        string webRootPath = _enviro.WebRootPath;
                        string filePath = Path.Combine(webRootPath, "images", path);
                            if (System.IO.File.Exists(filePath))
                            {
                                // 刪除檔案
                                System.IO.File.Delete(filePath);
                            }
                        }
                        
                    }
                }
                //新增輪播圖
                foreach (IFormFile photo in pro.photos)
                {
                    ProductPhotoList photoList = new ProductPhotoList();
                    string photoName = Guid.NewGuid().ToString() + ".jpg";//用Guid產生一個系統上不會重複的代碼，重新命名圖片
                    photoList.ImagePath = photoName;
                    photoList.ProductId = pro.ProductId;
                    photo.CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
                    db.ProductPhotoLists.Add(photoList);
                 }
            }
            //存入TripDetail
            foreach (TripDetailText t in pro.triptest)
            {
                TripDetail tripDb = db.TripDetails.FirstOrDefault(trip => trip.TripDetailId == t.TripDetailId);
                tripDb.TripDay = t.TripDay;
                tripDb.TripDetail1 = t.TripDetail;
               
                //照片
                if (t.photo != null)
                {
                    //先刪除舊圖
                    if (tripDb.ImagePath != null)
                    {
                        string oldImagePath = db.TripDetails.Where(trip => trip.TripDetailId == t.TripDetailId).Select(t => t.ImagePath).FirstOrDefault();
                        if (oldImagePath != null)
                        {
                            string webRootPath = _enviro.WebRootPath;
                            string filePath = Path.Combine(webRootPath, "images", oldImagePath);
                            if (System.IO.File.Exists(filePath))
                            {
                                // 刪除檔案
                                System.IO.File.Delete(filePath);
                            }
                        }

                    }
                    //存入新圖
                    string photoName = Guid.NewGuid().ToString() + ".jpg";//用Guid產生一個系統上不會重複的代碼，重新命名圖片
                    t.photo.CopyTo(new FileStream(_enviro.WebRootPath + "/images/" + photoName, FileMode.Create));
                    tripDb.ImagePath = photoName;
                }
            }

            db.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
