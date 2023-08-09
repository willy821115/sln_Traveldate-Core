using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class CommentPhotoList
{
    public int CommentPhotoListId { get; set; }

    public int? CommentId { get; set; }

    public string? ImagePath { get; set; }

    public virtual CommentList? Comment { get; set; }
}
