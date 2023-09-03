using prj_Traveldate_Core.Models;
using prj_Traveldate_Core.Models.MyModels;
using System.Drawing;
using X.PagedList;

namespace prj_Traveldate_Core.ViewModels
{ 
    
    public class CForumListViewModel
    {
        
        public List<ReplyList>? replyList { get; set; }
        
        public List<CCountryAndCity> regions { get; set; } =new List<CCountryAndCity>();
        public List<Member>? members { get; set; }
        public List<LevelList>? level { get; set; }
        public List<CForumList_prodPhoto>? prodPhoto { get; set; } =new List<CForumList_prodPhoto>();
        public List<CCategoryAndTags> categories { get; set; } = new List<CCategoryAndTags>();
        public List<ScheduleList1>? schedules { get; set; }
        public List<ScheduleList>? schedulesForProd { get; set; }
        public List<CForumList_topTen>? topTen {  get; set; }
        public List<ProductTagList>? productTags { get; set; }
        public IPagedList<ScheduleList1> pages { get; set; }
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }
        public List<CTripStock>? stocks { get; set; }=new List<CTripStock>();


        //public List<Trip> topTenTrip { get; set; }
        //public int? ForumlistId { get; set; }   

    }
   public class CForumList_prodPhoto
    {
        public string prodPhotoPath { get; set; }
        public int prodId { get; set; }
    }
    public class CForumList_topTen
    {
        public int? forumlistid { get; set; }
        public decimal? totalPrice { get; set; }
        public string? title { get; set; }
        public int? prodId { get; set; }
        public string content { get; set; } 
    }
    public class CTripStock
    {
        public int id { get; set; }
        public double stock { get; set; }
        public string strStock { get; set; }
    } 
}
