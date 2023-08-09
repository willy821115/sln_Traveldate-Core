using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class CityList
{
    public int CityId { get; set; }

    public string? City { get; set; }

    public int? CountryId { get; set; }

    public virtual CountryList? Country { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
