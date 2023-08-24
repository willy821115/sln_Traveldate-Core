using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using System.Drawing;

namespace prj_Traveldate_Core.ViewModels
{ 
    
    public class CForumListViewModel
    {
        public List<ArticlePhoto>? forumList { get; set; }
        public List<ReplyList>? replyList { get; set; }
        public List<ArticlePhoto>? photos { get; set; }
        public List<CCountryAndCity> regions { get; set; } =new List<CCountryAndCity>();
        public List<Member>? members { get; set; }
        public List<LevelList>? level { get; set; }
        public List<CForumList_prodPhoto>? prodPhoto { get; set; } =new List<CForumList_prodPhoto>();
        public List<CCategoryAndTags> categories { get; set; } = new List<CCategoryAndTags>();
        public List<ScheduleList>? schedules { get; set; }
        public List<ScheduleList>? schedulesForProd { get; set; }
    }
   public class CForumList_prodPhoto
    {
        public string prodPhotoPath { get; set; }
        public int prodId { get; set; }
        
    }
}
