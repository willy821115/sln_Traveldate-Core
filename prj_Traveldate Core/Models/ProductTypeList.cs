using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class ProductTypeList
{
    public int ProductTypeId { get; set; }

    public string? ProductType { get; set; }

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
