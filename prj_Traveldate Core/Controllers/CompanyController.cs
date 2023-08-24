using Microsoft.AspNetCore.Mvc;
using prj_Traveldate_Core.Models;

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
            var datas = _db.Companies.Where(c => c.CompanyId == companyID).FirstOrDefault();

            return View(datas);
        }
        [HttpPost]
        public IActionResult Edit(Company edit) 
        {
        Company cDB =_db.Companies.FirstOrDefault(p=>p.CompanyId==edit.CompanyId);
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
            
            return RedirectToAction("Edit");
        }

        public IActionResult Password() 
        {
            int companyID = 1;
            var datas = _db.Companies.Where(c => c.CompanyId == companyID).FirstOrDefault();

            return View(datas);
            
        }
        [HttpPost]
        public IActionResult Password(Company edit)
        {
            Company cDB = _db.Companies.FirstOrDefault(p => p.CompanyId == edit.CompanyId);
            if (cDB != null)
            {
                cDB.Password = edit.Password;

                _db.SaveChanges();
            }
            return View();
        }
    }
}
