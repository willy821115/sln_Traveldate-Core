using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class ProductTagList
{
    public int ProductTagListId { get; set; }

    public int? ProductId { get; set; }

    public int? ProductTagDetailsId { get; set; }

    public virtual ProductList? Product { get; set; }

    public virtual ProductTagDetail? ProductTagDetails { get; set; }
}
