﻿using Microsoft.AspNetCore.Mvc;
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
                    select new { productName = p.ProductName, productType = p.ProductType.ProductType, city = p.City.City, status = p.Status.Status1, discontinued=p.Discontinued };
           CProductListViewModel model = new CProductListViewModel();
            CProductFactory factory = new CProductFactory();
            model.status = factory.loadStauts();
            model.types = factory.loadTypes();
            model.list = new List<CProductWrap>();
            foreach (var item in q)
            {
                CProductWrap cProductWrap = new CProductWrap();
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
            return View(list);
        }
        [HttpPost]
        public IActionResult Create(CProductWrap pro)
         {
            return RedirectToAction("List");
        }



        public IActionResult Edit() 
        {
            CProductWrap list = new CProductWrap();
            CProductFactory factory = new CProductFactory();
            list.categoryAndTags = factory.loadCategories();
            list.countries = factory.loadCountries();
            list.cities = factory.loadCities();
            list.types = factory.loadTypes();
            return View(list);
        }
    }
}
