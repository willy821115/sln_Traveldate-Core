using System;
using System.Collections.Generic;

namespace prj_Traveldate_Core.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? Gender { get; set; }

    public string? Idnumber { get; set; }

    public DateTime? BirthDate { get; set; }

    public int? CityId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? LevelId { get; set; }

    public int? Discount { get; set; }

    public bool? Verified { get; set; }

    public bool? Enable { get; set; }

    public byte[]? Photo { get; set; }

    public string? ImagePath { get; set; }

    public virtual CityList? City { get; set; }

    public virtual ICollection<CommentList> CommentLists { get; set; } = new List<CommentList>();

    public virtual ICollection<Companion> Companions { get; set; } = new List<Companion>();

    public virtual ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<ForumList> ForumLists { get; set; } = new List<ForumList>();

    public virtual LevelList? Level { get; set; }

    public virtual ICollection<LikeList> LikeLists { get; set; } = new List<LikeList>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ReplyList> ReplyLists { get; set; } = new List<ReplyList>();
}
