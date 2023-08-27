using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class CouponList
{
    public int CouponListId { get; set; }

    public string? CouponName { get; set; }

    public decimal? Discount { get; set; }

    public byte[]? Photo { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public string? ImagePath { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
