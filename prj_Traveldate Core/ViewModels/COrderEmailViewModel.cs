using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;

namespace prj_Traveldate_Core.ViewModels
{
    public class COrderEmailViewModel
    {
        //public string title { get; set; }
        public string userName { get; set; }
        //public string content1 { get; set; }
        //public string content2 { get; set; }
        //public string content3 { get; set; }
        //public string buttonText { get; set; }
        public string buttonLink { get; set; }
        //public string contentFooter { get; set; }
        //public string contentTitle { get; set; }
        public List<OrderDetail> ods { get; set; } 
        public List<COrderMail> orders { get; set; } 
        public int coupon { get; set; }
        public int point { get; set; }
        public int total { get; set; }
    }
}
