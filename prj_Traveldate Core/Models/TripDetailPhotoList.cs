using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class TripDetailPhotoList
{
    public int TripDetailPhotoListId { get; set; }

    public int? TripDetailId { get; set; }

    public string? ImagePath { get; set; }
}
