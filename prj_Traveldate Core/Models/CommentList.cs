using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class CommentList
{
    public int CommentId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? CommentScore { get; set; }

    public int? MemberId { get; set; }

    public DateTime? Date { get; set; }

    public int? ProductId { get; set; }

    public virtual ICollection<CommentPhotoList> CommentPhotoLists { get; set; } = new List<CommentPhotoList>();

    public virtual Member? Member { get; set; }

    public virtual ProductList? Product { get; set; }
}
