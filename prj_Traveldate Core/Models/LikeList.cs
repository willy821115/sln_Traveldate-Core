using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class LikeList
{
    public int LikeId { get; set; }

    public int? MemberId { get; set; }

    public int? ForumId { get; set; }

    public bool? IsLike { get; set; }

    public virtual ForumList? Forum { get; set; }

    public virtual Member? Member { get; set; }
}
