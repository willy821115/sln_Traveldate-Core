using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.Controllers
{
    public class CompanyController : CompanySuperController
    {
        private TraveldateContext _db = null;

        public CompanyController()
        {
            _db = new TraveldateContext();
            //HttpContext.Session.SetInt32(CDictionary.SK_COMPANYID, 1);
            //companyID=  (int)HttpContext.Session.GetInt32(CDictionary.SK_COMPANYID);
        }
        public IActionResult Edit()
        {
            int companyID = 1;
            CCompanyWrap cCompany= new CCompanyWrap();
            CProductFactory productFactory = new CProductFactory();
            cCompany.company = _db.Companies.Where(c => c.CompanyId == companyID).FirstOrDefault();
            cCompany.cities=productFactory.loadCities();
            cCompany.country=productFactory.loadCountries();
            return View(cCompany);
        }
        [HttpPost]
        public IActionResult Edit(CCompanyWrap edit) 
        {
            Thread.Sleep(1000);
            Company cDB =_db.Companies.FirstOrDefault(p=>p.CompanyId==edit.CompanyId);
            try 
            {
           
            if (cDB != null) 
            {
                cDB.Address = edit.Address;
                cDB.City = edit.City;
                cDB.Email= edit.Email;
                cDB.Phone = edit.Phone;
                cDB.PostalCode = edit.PostalCode;
                cDB.Country = edit.Country;
                cDB.ServerDescription = edit.ServerDescription;
                cDB.Address=edit.Address;
                cDB.TaxIdNumber = edit.TaxIdNumber;
                cDB.Url = edit.Url;
                cDB.Contact=edit.Contact;
                cDB.Principal=edit.Principal;
                cDB.Title=edit.Title;
                
                _db.SaveChanges();
            }
            
            return RedirectToAction("List", "Dashboard");
            }
            catch (Exception ex) 
            {
                return RedirectToAction("List", "Dashboard");
            }
        }

        public IActionResult Password() 
        {
            int companyID = 1;
            CCompanyWrap cCompany = new CCompanyWrap();
            cCompany.company = _db.Companies.Where(c => c.CompanyId == companyID).FirstOrDefault();

            return View(cCompany);
            
        }
        [HttpPost]
        public IActionResult Password(CCompanyWrap edit)
        {
            Thread.Sleep(1000);
            if (edit.Password == edit.newPasswordCheck)
            {
                
                Company cDB = _db.Companies.FirstOrDefault(p => p.CompanyId == edit.CompanyId);
                try
                {

                    if (cDB != null)
                    {
                        cDB.Password = edit.Password;

                        _db.SaveChanges();
                    }
                    return RedirectToAction("List", "Dashboard");
                }

                catch (Exception ex) 
                {
                return RedirectToAction("List", "Dashboard");
                }
                
            }
            else 
            {
                ViewBag.CompanyPasswordAlarm = "新密碼與確認密碼不一致";
                return View();
            }
         
            
        }
    }
}
