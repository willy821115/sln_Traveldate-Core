using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using System.Drawing;

namespace prj_Traveldate_Core.ViewModels
{
    public class CForumListViewModel
    {
        public List<ForumList>? forumList { get; set; }
        public List<ReplyList>? replyList { get; set; }
        public List<ArticlePhoto>? photos { get; set; }
        public List<CCountryAndCity> regions { get; set; } =new List<CCountryAndCity>();
        public List<Member>? members { get; set; }
        public List<LevelList>? level { get; set; }
        public List<CForumList_prodPhoto>? prodPhoto { get; set; }
        public List<CCategoryAndTags> categories { get; set; } = new List<CCategoryAndTags>();
        public List<ScheduleList>? schedules { get; set; }  
    }
    public class CForumList_prodPhoto
    {
        public int prodId { get; set; }
        public byte[] prodPhoto { get; set; }
    }
}
