namespace prj_Traveldate_Core.Models.MyModels
{
    public class CCountryAndCity
    {
        public string? country { get; set; }
        public IEnumerable<string>? citys { get; set; }
        public string city { get; set; }
    }
    public class ScheduleList1
    {
        public int? forumListId { get; set; }
        public List<Trip?> trips { get; set; }
        public ForumList? ForumList { get; set; }
        public List<ProductTagList> productTags { get; set; }
 public int ProductTagDetailsId { get; set; }
     
    }
    
}
