using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class ProductList
{
    public int ProductId { get; set; }

    public int CompanyId { get; set; }

    public string? ProductName { get; set; }

    public int? CityId { get; set; }

    public string? Description { get; set; }

    public int? ProductTypeId { get; set; }

    public int? StatusId { get; set; }

    public string? PlanName { get; set; }

    public string? PlanDescription { get; set; }

    public bool? Discontinued { get; set; }

    public string? Outline { get; set; }

    public string? OutlineForSearch { get; set; }

    public string? Address { get; set; }

    public virtual CityList? City { get; set; }

    public virtual ICollection<CommentList> CommentLists { get; set; } = new List<CommentList>();

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<ProductPhotoList> ProductPhotoLists { get; set; } = new List<ProductPhotoList>();

    public virtual ICollection<ProductTagList> ProductTagLists { get; set; } = new List<ProductTagList>();

    public virtual ProductTypeList? ProductType { get; set; }

    public virtual Status? Status { get; set; }

    public virtual ICollection<TripDetail> TripDetails { get; set; } = new List<TripDetail>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
