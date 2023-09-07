using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CCreatArticleViewModel
    {
        public ForumList forum {  get; set; }
        //public List<ProductList> prods { get; set; }
        public ScheduleList schedule { get; set; }
        public List<int> tripIds { get; set; }
        public Trip trips { get; set; }
        public string isSave { get; set; }
        public string isPublish { get; set; }
        public string isDemo{ get; set; }
        public List<ScheduleList> schedules { get; set; }
    }
}
