using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.ViewModels
{
    public class CPlatformViewModel
    {
        public List<CProductWrap> product { get; set; }
        public List<ProductTypeList> types { get; set; }
        public List<string> status { get; set; }

        public string? txtKeyword { get; set; }

        public List<Company> company { get; set; }

    }
}
