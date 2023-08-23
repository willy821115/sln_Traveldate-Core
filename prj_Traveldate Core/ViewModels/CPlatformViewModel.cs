using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.ViewModels
{
    public class CPlatformViewModel
    {
        public List<CProductWrap> product { get; set; }
        public List<CCouponWrap> coupon { get; set; }
        public List<ProductTypeList> types { get; set; }
        public List<string> status { get; set; }

        public string? txtKeyword { get; set; }

        public List<Company> company { get; set; }
        public string companySelect { get; set; }
        public string productTypeSelect { get; set; }
        public string statusSelect { get; set; }

        public List<CPlatformMemViewModel> Members { get; set; }
        public List<CPlatformMemViewModel> Companies { get; set; }

        public string SelectedCompany { get; set; }
        public string SelectedProductType { get; set; }
        public string SelectedStatus { get; set; }

    }
}
