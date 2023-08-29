namespace prj_Traveldate_Core.Controllers
{
    internal class CForumListViewModel2
    {
        public int ForumListId { get; set; }
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReleaseDatetime { get; set; }
        public int? Likes { get; set; }
        public int? Watches { get; set; }
        public string? Content { get; set; }
        public bool? IsPublish { get; internal set; }
    }
}