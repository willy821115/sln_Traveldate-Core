using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class ProductPhotoList
{
    public int ProductPhotoListId { get; set; }

    public int? ProductId { get; set; }

    public byte[]? Photo { get; set; }

    public string? ImagePath { get; set; }

    public virtual ProductList? Product { get; set; }
}
