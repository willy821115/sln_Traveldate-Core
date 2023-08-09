using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class TripDetail
{
    public int TripDetailId { get; set; }

    public int? TripId { get; set; }

    public string? TripDetail1 { get; set; }

    public int? ProductPhotoListId { get; set; }

    public int? TripDay { get; set; }

    public virtual ProductPhotoList? ProductPhotoList { get; set; }

    public virtual Trip? Trip { get; set; }
}
