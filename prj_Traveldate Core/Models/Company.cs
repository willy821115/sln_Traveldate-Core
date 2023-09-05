using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string? TaxIdNumber { get; set; }

    public string? CompanyName { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Url { get; set; }

    public string? Principal { get; set; }

    public string? Contact { get; set; }

    public string? Title { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? ServerDescription { get; set; }

    public bool? Enable { get; set; }

    public virtual ICollection<ProductList> ProductLists { get; set; } = new List<ProductList>();
}
