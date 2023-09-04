using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class ForumList
{
    public int ForumListId { get; set; }

    public int? MemberId { get; set; }

    public string? Title { get; set; }

    public DateTime? DueDate { get; set; }

    public string? Content { get; set; }

    public DateTime? ReleaseDatetime { get; set; }

    public int? Watches { get; set; }

    public bool? IsPublish { get; set; }

    public int? Likes { get; set; }

    public virtual ICollection<ArticlePhoto> ArticlePhotos { get; set; } = new List<ArticlePhoto>();

    public virtual ICollection<LikeList> LikeLists { get; set; } = new List<LikeList>();

    public virtual Member? Member { get; set; }

    public virtual ICollection<ReplyList> ReplyLists { get; set; } = new List<ReplyList>();

    public virtual ICollection<ScheduleList> ScheduleLists { get; set; } = new List<ScheduleList>();
}
