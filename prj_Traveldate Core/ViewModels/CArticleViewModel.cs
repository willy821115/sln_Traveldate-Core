using prj_Traveldate_Core.Models;

namespace prj_Traveldate_Core.ViewModels
{
    public class CArticleViewModel
    {
        public ForumList? forum { get; set; }
        public List<ReplyList>? replys { get; set; }
       
        public Member? member { get; set; }
        public List<string>? fforumAddress { get; set; }
    }
}
