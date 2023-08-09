using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class ArticlePhoto
{
    public int ArticlePhotoId { get; set; }

    public int? ForumListId { get; set; }

    public string? ImagePath { get; set; }

    public virtual ForumList? ForumList { get; set; }
}
