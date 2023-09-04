namespace prj_Traveldate_Core.Models.MyModels
{
    public class CCategoryAndTags
    {
        public string category { get; set; }
        public List<string> tags { get; set; }
        
        public List<ProductTagDetail> productTags { get; set; }
       
    }
}
