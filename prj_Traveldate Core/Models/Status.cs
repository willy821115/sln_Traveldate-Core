using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string? Status1 { get; set; }

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
