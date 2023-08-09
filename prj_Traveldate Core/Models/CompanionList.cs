using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class CompanionList
{
    public int CompanionListId { get; set; }

    public int? OrderDetailsId { get; set; }

    public int? CompanionId { get; set; }

    public virtual Companion? Companion { get; set; }

    public virtual OrderDetail? OrderDetails { get; set; }
}
