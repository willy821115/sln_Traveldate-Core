using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int MemberId { get; set; }

    public DateTime? Datetime { get; set; }

    public int? CouponListId { get; set; }

    public int? Discount { get; set; }

    public string? PaymentId { get; set; }

    public bool? IsCart { get; set; }

    public virtual CouponList? CouponList { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual EcpayOrder? Payment { get; set; }
}
