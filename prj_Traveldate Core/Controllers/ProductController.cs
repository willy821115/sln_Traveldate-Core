using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using prj_Traveldate_Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prj_Traveldate_Core.Controllers
{
    public class ProductController : CompanySuperController
    {
        
        private int companyID = 0;
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
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
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
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
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
            Thread.Sleep(1000);
            TraveldateContext db = new TraveldateContext();
            //存入ProductLists
            ProductList save = new ProductList();
            try
            {

                save.CompanyId = pro.CompanyId;
                save.ProductName = pro.ProductName;
                save.CityId = pro.CityId;
                save.Description = pro.Description;
                save.ProductTypeId = pro.ProductTypeId;
                save.StatusId = 2;
                save.PlanName = pro.PlanName;
                save.PlanDescription = pro.PlanDescription;
                save.Discontinued = false;
                save.Outline = pro.Outline;
                save.OutlineForSearch = pro.OutlineForSearch;
                save.Address = pro.Address;

                db.ProductLists.Add(save);
                db.SaveChanges();
                //獲取ProductID
                productID = db.ProductLists.Where(p => p.ProductName == pro.ProductName).Select(p => Convert.ToInt32(p.ProductId)).FirstOrDefault();
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
                //輪播圖
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
            catch (Exception ex) 
            {
                return RedirectToAction("List");
            }
            
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
            list.CtripDetail = db.TripDetails.Where(t => t.ProductId == productID).OrderBy(t=>t.TripDay).ToList();
            return View(list);
        }

        [HttpPost]

        public IActionResult Edit(CProductWrap pro) 
        {
            Thread.Sleep(1000);
            TraveldateContext db = new TraveldateContext();
            ProductList proDb=db.ProductLists.FirstOrDefault(p=>p.ProductId==pro.ProductId);
            //存入ProductList
            try
            { 
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
                else 
                {
                    tripDb.ImagePath = t.ImagePath;
                }

            }

            db.SaveChanges();
            return RedirectToAction("List");
            }
            catch (Exception ex) 
            {
                return RedirectToAction("List");
            }
        }

        public IActionResult queryByType(int typeID)
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            TraveldateContext db = new TraveldateContext();
            CProductFactory pro = new CProductFactory();
            var q = db.ProductLists.Where(p => p.ProductTypeId == typeID && p.CompanyId == companyID).Select(p => new
            {
                productName = p.ProductName,
                productType = p.ProductType.ProductType,
                cityName = p.City.City,
                productStatus = p.Status.Status1,
                discontinued = (bool)p.Discontinued ? "下架" : "上架",
                productID =p.ProductId
            }) ;
            return Json(q);
        }

        public IActionResult queryByStatus(int statusID) 
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            TraveldateContext db = new TraveldateContext();
            CProductFactory pro = new CProductFactory();
            var q = db.ProductLists.Where(p => p.StatusId== statusID && p.CompanyId == companyID).Select(p => new
            {
                productName = p.ProductName,
                productType = p.ProductType.ProductType,
                cityName = p.City.City,
                productStatus = p.Status.Status1,
                Discontinued = (bool)p.Discontinued ? "下架" : "上架",
                productID = p.ProductId
            });
            return Json(q);
        }
        public IActionResult SaleOperate(int productID)
        {
            companyID = Convert.ToInt32(HttpContext.Session.GetString(CDictionary.SK_LOGGEDIN_COMPANY));
            TraveldateContext db = new TraveldateContext();
            CProductFactory pro = new CProductFactory();
            var q = db.ProductLists.FirstOrDefault(p => p.ProductId == productID);
            q.Discontinued = !q.Discontinued;
            db.SaveChanges();
            var q2 = db.ProductLists.Where(p=>p.CompanyId == companyID).Select(p => new
            {
                productName = p.ProductName,
                productType = p.ProductType.ProductType,
                cityName = p.City.City,
                productStatus = p.Status.Status1,
                Discontinued = (bool)p.Discontinued ? "下架" : "上架",
                productID = p.ProductId
            });
            return Json(q2.ToList());

        }

        public IActionResult ProductPreview(CProductWrap pro) 
        {
            TraveldateContext db = new TraveldateContext();
            ProgramViewModel data = new ProgramViewModel();
             List <string> previewPhoto = new List<string>();
            List<string> previewTripPhoto = new List<string>();
            List<string>TripDescription = new List<string>();
            List<string> PlanDescriptionSplit = new List<string>();
            List<string> OutlineSplit = new List<string>();
            //將倫播圖轉換成Base64编码
            try 
            {
           
            if (pro.photos != null && pro.photos.Count > 0) 
            {
            
                foreach (var photo in pro.photos) 
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        photo.CopyTo(memoryStream);
                         // 将文件内容转换为Base64编码
                        var base64String = Convert.ToBase64String(memoryStream.ToArray());
                        previewPhoto.Add(base64String);
                    }
                }
            }
            
            if (pro.triptest != null && pro.triptest.Count > 0)
            {
                //將TripDetail圖轉換成Base64编码
                foreach (var photo in pro.triptest)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        photo.photo.CopyTo(memoryStream);
                        // 将文件内容转换为Base64编码
                        var base64String = Convert.ToBase64String(memoryStream.ToArray());
                        previewTripPhoto.Add(base64String);
                    }
                }
                foreach (var description in pro.triptest) 
                {
                    TripDescription.Add(description.TripDetail);
                }
            }
            
            //PlanDescription依照換行切割
            if (pro.PlanDescription != null)
            {
                string[] planDetails = pro.PlanDescription.Split('\n');
                PlanDescriptionSplit= planDetails.ToList();
            }
            //Outline依照換行切割
            if (pro.PlanDescription != null)
            {
                string[] outlines = pro.Outline.Split('\n');
                OutlineSplit = outlines.ToList();
            }
            //將CityID轉成CityName
            string CityName = db.CityLists.Where(c => c.CityId == pro.CityId).Select(c => c.City).FirstOrDefault();
            //將TagID轉換成TagName
            List<string> TagNames= new List<string>();
            foreach (var tagID in pro.Tags) 
            {
                string name = db.ProductTagDetails.Where(t => t.ProductTagDetailsId == tagID).Select(t => t.ProductTagDetailsName).FirstOrDefault();
                TagNames.Add(name);
            }
            data.program.fPhotoPath = previewPhoto;
            data.product.ProductName = pro.ProductName;
            data.product.Description = pro.Description;
            data.product.PlanName = pro.PlanName;
            data.program.fPlanDescription = PlanDescriptionSplit;
            data.product.Address = pro.Address;
            data.program.fOutline = OutlineSplit;
            data.city.City = CityName;
            data.program.fTripDetails = TripDescription;
            data.program.fTTripPhotoList = previewTripPhoto;
            data.program.fProdTags = TagNames;
           

            return View("ProductPreview", data);
            }
            catch (Exception ex) 
            {
                return View("ProductPreview", data);
            }
        }
    }
    
}
