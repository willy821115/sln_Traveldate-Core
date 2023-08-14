using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class ReplyList
{
    public int ReplyId { get; set; }

    public int? ForumListId { get; set; }

    public int? ReplyToId { get; set; }

    public string? Content { get; set; }

    public int? MemberId { get; set; }

    public DateTime? ReplyTime { get; set; }

    public virtual ForumList? ForumList { get; set; }

    public virtual ICollection<ReplyList> InverseReplyTo { get; set; } = new List<ReplyList>();

    public virtual Member? Member { get; set; }

    public virtual ReplyList? ReplyTo { get; set; }
}
