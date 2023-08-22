using prj_Traveldate_Core.Models.MyModels;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core.Mvc;


namespace prj_Traveldate_Core.ViewModels
{
    public class CSearchListViewModel
    {
        public List<CFilteredProductItem> filterProducts { get; set; } = new List<CFilteredProductItem>();
        public List<CCategoryAndTags> categoryAndTags { get; set; } = new List<CCategoryAndTags>();
        public List<CCountryAndCity> countryAndCities { get; set; } = new List<CCountryAndCity>();
        //public List<string> fTags { get; set; } = new List<string>();
        public List<string> types { get; set; } = new List<string>();
        public IPagedList<CFilteredProductItem> pages { get; set; }
        public List<CFilteredProductItem> pageFilterProducts { get; set; } = new List<CFilteredProductItem>();
        public int currentPage { get; set; }
        
    }
}
