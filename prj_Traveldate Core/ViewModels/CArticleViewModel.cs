using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using RazorEngine.Compilation.ImpromptuInterface;

namespace prj_Traveldate_Core.ViewModels
{
    public class CArticleViewModel
    {
        public ForumList? forum { get; set; }
        public List<ReplyList>? replys { get; set; }
        public Member? member { get; set; }
        public List<string>? fforumAddress { get; set; }
        public ScheduleList1 schedule { get; set; }
       public List<string?> trip_Tags { get; set;}
       public List<Trip> articleTrips { get; set; }
        public List<CArticle_imgs> img_Paths { get; set; }
      public List<CCommentScore> avgCommentScores { get; set; }
        public List<LikeList> likes { get; set; }
        public int? returnStatus { get; set; }
    }
    public class CArticle_imgs
    {
        public int img_prodId { get; set; }
        public List<string> imgPaths { get; set; }
    }
    public class CCommentScore
    {
        public int? commentProdId { get; set; }
        public double? commentScore { get; set; }
    }
}
