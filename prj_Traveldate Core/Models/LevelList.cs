using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class LevelList
{
    public int LevelId { get; set; }

    public int? Standard { get; set; }

    public string? Level { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
