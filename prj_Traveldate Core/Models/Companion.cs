using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class Companion
{
    public int CompanionId { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? Idnumber { get; set; }

    public string? Phone { get; set; }

    public int? MemberId { get; set; }

    public DateTime? BirthDate { get; set; }

    public virtual ICollection<CompanionList> CompanionLists { get; set; } = new List<CompanionList>();

    public virtual Member? Member { get; set; }
}
