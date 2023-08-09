using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class OrderDetail
{
    public int OrderDetailsId { get; set; }

    public int OrderId { get; set; }

    public int? Quantity { get; set; }

    public int? StatusId { get; set; }

    public int? TripId { get; set; }

    public decimal? SellingPrice { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<CompanionList> CompanionLists { get; set; } = new List<CompanionList>();

    public virtual Order Order { get; set; } = null!;

    public virtual OrderStatusList? Status { get; set; }

    public virtual Trip? Trip { get; set; }
}
