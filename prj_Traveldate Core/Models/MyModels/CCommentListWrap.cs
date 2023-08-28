namespace prj_Traveldate_Core.Models.MyModels
{
    public class CCommentListWrap
    {
        public int CommentId { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public int? CommentScore { get; set; }

        public int? MemberId { get; set; }

        public DateTime? Date { get; set; }

        public int? ProductId { get; set; }

        public string? ProductName { get; set; }

        public List<IFormFile>? photos { get; set; }
        public string? ImagePath { get; set; }

    }
}
