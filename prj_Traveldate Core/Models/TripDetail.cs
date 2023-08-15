using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class TripDetail
{
    public int TripDetailId { get; set; }

    public int? ProductId { get; set; }

    public string? TripDetail1 { get; set; }

    public int? TripDay { get; set; }

    public string? ImagePath { get; set; }

    public virtual ProductList? Product { get; set; }
}
