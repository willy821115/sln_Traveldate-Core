using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class ProductCategory
{
    public int ProductCategoryId { get; set; }

    public string? ProductCategoryName { get; set; }

    public virtual ICollection<ProductTagDetail> ProductTagDetails { get; set; } = new List<ProductTagDetail>();
}
