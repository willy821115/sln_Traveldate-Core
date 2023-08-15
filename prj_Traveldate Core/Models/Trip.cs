using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class Trip
{
    public int TripId { get; set; }

    public int ProductId { get; set; }

    public DateTime? Date { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? MinNum { get; set; }

    public int? MaxNum { get; set; }

    public decimal? Discount { get; set; }

    public DateTime? DiscountExpirationDate { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductList Product { get; set; } = null!;

    public virtual ICollection<ScheduleList> ScheduleLists { get; set; } = new List<ScheduleList>();
}
