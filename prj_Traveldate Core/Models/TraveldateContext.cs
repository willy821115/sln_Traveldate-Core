using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace prj_Traveldate_Core.Models;

public partial class TraveldateContext : DbContext
{
    public TraveldateContext()
    {
    }

    public TraveldateContext(DbContextOptions<TraveldateContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArticlePhoto> ArticlePhotos { get; set; }

    public virtual DbSet<CityList> CityLists { get; set; }

    public virtual DbSet<CommentList> CommentLists { get; set; }

    public virtual DbSet<CommentPhotoList> CommentPhotoLists { get; set; }

    public virtual DbSet<Companion> Companions { get; set; }

    public virtual DbSet<CompanionList> CompanionLists { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CountryList> CountryLists { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<CouponList> CouponLists { get; set; }

    public virtual DbSet<EcpayOrder> EcpayOrders { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<ForumList> ForumLists { get; set; }

    public virtual DbSet<LevelList> LevelLists { get; set; }

    public virtual DbSet<LikeList> LikeLists { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatusList> OrderStatusLists { get; set; }

    public virtual DbSet<PaymentList> PaymentLists { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductList> ProductLists { get; set; }

    public virtual DbSet<ProductPhotoList> ProductPhotoLists { get; set; }

    public virtual DbSet<ProductTagDetail> ProductTagDetails { get; set; }

    public virtual DbSet<ProductTagList> ProductTagLists { get; set; }

    public virtual DbSet<ProductTypeList> ProductTypeLists { get; set; }

    public virtual DbSet<ReplyList> ReplyLists { get; set; }

    public virtual DbSet<ScheduleList> ScheduleLists { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<TripDetail> TripDetails { get; set; }

    public virtual DbSet<TripDetailPhotoList> TripDetailPhotoLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Traveldate;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticlePhoto>(entity =>
        {
            entity.Property(e => e.ArticlePhotoId).HasColumnName("ArticlePhotoID");
            entity.Property(e => e.ForumListId).HasColumnName("ForumListID");
            entity.Property(e => e.ImagePath).HasMaxLength(50);

            entity.HasOne(d => d.ForumList).WithMany(p => p.ArticlePhotos)
                .HasForeignKey(d => d.ForumListId)
                .HasConstraintName("FK_ArticlePhotos_ForumList");
        });

        modelBuilder.Entity<CityList>(entity =>
        {
            entity.HasKey(e => e.CityId);

            entity.ToTable("CityList");

            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CountryId).HasColumnName("CountryID");

            entity.HasOne(d => d.Country).WithMany(p => p.CityLists)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("FK_CityList_CountryList");
        });

        modelBuilder.Entity<CommentList>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK_評論清單");

            entity.ToTable("CommentList");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Member).WithMany(p => p.CommentLists)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_CommentList_Member");

            entity.HasOne(d => d.Product).WithMany(p => p.CommentLists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_CommentList_ProductList");
        });

        modelBuilder.Entity<CommentPhotoList>(entity =>
        {
            entity.ToTable("CommentPhotoList");

            entity.Property(e => e.CommentPhotoListId).HasColumnName("CommentPhotoListID");
            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.ImagePath).HasMaxLength(50);

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentPhotoLists)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_CommentPhotoList_CommentList");
        });

        modelBuilder.Entity<Companion>(entity =>
        {
            entity.HasKey(e => e.CompanionId).HasName("PK_同行旅客資料");

            entity.ToTable("Companion");

            entity.Property(e => e.CompanionId).HasColumnName("CompanionID");
            entity.Property(e => e.BirthDate).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Idnumber)
                .HasMaxLength(50)
                .HasColumnName("IDNumber");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Member).WithMany(p => p.Companions)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_Companion_Member");
        });

        modelBuilder.Entity<CompanionList>(entity =>
        {
            entity.ToTable("CompanionList");

            entity.Property(e => e.CompanionListId).HasColumnName("CompanionListID");
            entity.Property(e => e.CompanionId).HasColumnName("CompanionID");
            entity.Property(e => e.OrderDetailsId).HasColumnName("OrderDetailsID");

            entity.HasOne(d => d.Companion).WithMany(p => p.CompanionLists)
                .HasForeignKey(d => d.CompanionId)
                .HasConstraintName("FK_CompanionList_Companion");

            entity.HasOne(d => d.OrderDetails).WithMany(p => p.CompanionLists)
                .HasForeignKey(d => d.OrderDetailsId)
                .HasConstraintName("FK_CompanionList_OrderDetails");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK_公司資料表_1");

            entity.ToTable("Company");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CompanyName).HasMaxLength(50);
            entity.Property(e => e.Contact).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PostalCode).HasMaxLength(50);
            entity.Property(e => e.Principal).HasMaxLength(50);
            entity.Property(e => e.ServerDescription).HasMaxLength(50);
            entity.Property(e => e.TaxIdNumber)
                .HasMaxLength(50)
                .HasColumnName("Tax_ID_Number");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Url)
                .HasMaxLength(50)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<CountryList>(entity =>
        {
            entity.HasKey(e => e.CountryId);

            entity.ToTable("CountryList");

            entity.Property(e => e.CountryId)
                .ValueGeneratedNever()
                .HasColumnName("CountryID");
            entity.Property(e => e.Country).HasMaxLength(50);
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.ToTable("Coupon");

            entity.Property(e => e.CouponId).HasColumnName("CouponID");
            entity.Property(e => e.CouponListId).HasColumnName("CouponListID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.CouponList).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.CouponListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Coupon_CouponList");

            entity.HasOne(d => d.Member).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_會員優惠券_會員資料表");
        });

        modelBuilder.Entity<CouponList>(entity =>
        {
            entity.HasKey(e => e.CouponListId).HasName("PK_優惠券清單");

            entity.ToTable("CouponList");

            entity.Property(e => e.CouponListId).HasColumnName("CouponListID");
            entity.Property(e => e.CouponName).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.ImagePath).HasMaxLength(50);
        });

        modelBuilder.Entity<EcpayOrder>(entity =>
        {
            entity.HasKey(e => e.MerchantTradeNo);

            entity.Property(e => e.MerchantTradeNo).HasMaxLength(50);
            entity.Property(e => e.MemberId)
                .HasMaxLength(50)
                .HasColumnName("MemberID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentType).HasMaxLength(50);
            entity.Property(e => e.PaymentTypeChargeFee).HasMaxLength(50);
            entity.Property(e => e.RtnMsg).HasMaxLength(50);
            entity.Property(e => e.TradeDate).HasMaxLength(50);
            entity.Property(e => e.TradeNo).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.EmployeeAccount).HasMaxLength(50);
            entity.Property(e => e.EmployeeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("EmployeeID");
            entity.Property(e => e.EmployeePassword).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("Favorite");

            entity.Property(e => e.FavoriteId).HasColumnName("FavoriteID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Member).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_我的收藏_會員資料表");

            entity.HasOne(d => d.Product).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Favorite_ProductList");
        });

        modelBuilder.Entity<ForumList>(entity =>
        {
            entity.ToTable("ForumList");

            entity.Property(e => e.ForumListId).HasColumnName("ForumListID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.IsPublish).HasColumnName("isPublish");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ReleaseDatetime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.Watches).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Member).WithMany(p => p.ForumLists)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_ForumList_Member");
        });

        modelBuilder.Entity<LevelList>(entity =>
        {
            entity.HasKey(e => e.LevelId);

            entity.ToTable("LevelList");

            entity.Property(e => e.LevelId)
                .ValueGeneratedNever()
                .HasColumnName("LevelID");
            entity.Property(e => e.Level).HasMaxLength(50);
        });

        modelBuilder.Entity<LikeList>(entity =>
        {
            entity.HasKey(e => e.LikeId);

            entity.ToTable("LikeList");

            entity.Property(e => e.LikeId).HasColumnName("LikeID");
            entity.Property(e => e.ForumId).HasColumnName("ForumID");
            entity.Property(e => e.IsLike).HasColumnName("isLike");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.Forum).WithMany(p => p.LikeLists)
                .HasForeignKey(d => d.ForumId)
                .HasConstraintName("FK_LikeList_ForumList");

            entity.HasOne(d => d.Member).WithMany(p => p.LikeLists)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_LikeList_Member");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK_會員資料表_1");

            entity.ToTable("Member");

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.BirthDate).HasColumnType("datetime");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Idnumber)
                .HasMaxLength(50)
                .HasColumnName("IDNumber");
            entity.Property(e => e.ImagePath).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(d => d.City).WithMany(p => p.Members)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Member_CityList");

            entity.HasOne(d => d.Level).WithMany(p => p.Members)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Member_LevelList");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_會員訂單");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CouponListId).HasColumnName("CouponListID");
            entity.Property(e => e.Datetime).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.PaymentId)
                .HasMaxLength(50)
                .HasColumnName("PaymentID");

            entity.HasOne(d => d.CouponList).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CouponListId)
                .HasConstraintName("FK_Orders_CouponList");

            entity.HasOne(d => d.Member).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Member");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Orders_EcpayOrders1");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailsId);

            entity.Property(e => e.OrderDetailsId).HasColumnName("OrderDetailsID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.SellingPrice).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.TripId).HasColumnName("TripID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_會員訂單清單_會員訂單");

            entity.HasOne(d => d.Status).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_OrderDetails_OrderStatusList");

            entity.HasOne(d => d.Trip).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_會員訂單清單_出團");
        });

        modelBuilder.Entity<OrderStatusList>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("OrderStatusList");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<PaymentList>(entity =>
        {
            entity.HasKey(e => e.PaymentId);

            entity.ToTable("PaymentList");

            entity.Property(e => e.PaymentId)
                .ValueGeneratedNever()
                .HasColumnName("PaymentID");
            entity.Property(e => e.Payment).HasMaxLength(50);
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId).HasName("PK_ProductTag");

            entity.ToTable("ProductCategory");

            entity.Property(e => e.ProductCategoryId)
                .ValueGeneratedNever()
                .HasColumnName("ProductCategoryID");
            entity.Property(e => e.ProductCategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<ProductList>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_公司商品清單");

            entity.ToTable("ProductList");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.PlanName).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(50);
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.City).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_ProductList_CityList");

            entity.HasOne(d => d.Company).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_公司商品清單_公司資料表1");

            entity.HasOne(d => d.ProductType).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("FK_ProductList_ProductTypeList");

            entity.HasOne(d => d.Status).WithMany(p => p.ProductLists)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_ProductList_Status");
        });

        modelBuilder.Entity<ProductPhotoList>(entity =>
        {
            entity.ToTable("ProductPhotoList");

            entity.Property(e => e.ProductPhotoListId).HasColumnName("ProductPhotoListID");
            entity.Property(e => e.ImagePath).HasMaxLength(50);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPhotoLists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductPhotoList_ProductList");
        });

        modelBuilder.Entity<ProductTagDetail>(entity =>
        {
            entity.HasKey(e => e.ProductTagDetailsId).HasName("PK_ProductTagDetails_1");

            entity.Property(e => e.ProductTagDetailsId).HasColumnName("ProductTagDetailsID");
            entity.Property(e => e.ProductCategoryId).HasColumnName("ProductCategoryID");
            entity.Property(e => e.ProductTagDetailsName).HasMaxLength(50);

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.ProductTagDetails)
                .HasForeignKey(d => d.ProductCategoryId)
                .HasConstraintName("FK_ProductTagDetails_ProductTag");
        });

        modelBuilder.Entity<ProductTagList>(entity =>
        {
            entity.ToTable("ProductTagList");

            entity.Property(e => e.ProductTagListId).HasColumnName("ProductTagListID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductTagDetailsId).HasColumnName("ProductTagDetailsID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductTagLists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductTagList_ProductList");

            entity.HasOne(d => d.ProductTagDetails).WithMany(p => p.ProductTagLists)
                .HasForeignKey(d => d.ProductTagDetailsId)
                .HasConstraintName("FK_ProductTagList_ProductTagDetails");
        });

        modelBuilder.Entity<ProductTypeList>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId);

            entity.ToTable("ProductTypeList");

            entity.Property(e => e.ProductTypeId)
                .ValueGeneratedNever()
                .HasColumnName("ProductTypeID");
            entity.Property(e => e.ProductType).HasMaxLength(50);
        });

        modelBuilder.Entity<ReplyList>(entity =>
        {
            entity.HasKey(e => e.ReplyId);

            entity.ToTable("ReplyList");

            entity.Property(e => e.ReplyId).HasColumnName("ReplyID");
            entity.Property(e => e.ForumListId).HasColumnName("ForumListID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ReplyToId).HasColumnName("ReplyToID");
            entity.Property(e => e.Replytime).HasColumnType("datetime");

            entity.HasOne(d => d.ForumList).WithMany(p => p.ReplyLists)
                .HasForeignKey(d => d.ForumListId)
                .HasConstraintName("FK_ReplyList_ForumList");

            entity.HasOne(d => d.Member).WithMany(p => p.ReplyLists)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReplyList_Member");

            entity.HasOne(d => d.ReplyTo).WithMany(p => p.InverseReplyTo)
                .HasForeignKey(d => d.ReplyToId)
                .HasConstraintName("FK_ReplyList_ReplyList");
        });

        modelBuilder.Entity<ScheduleList>(entity =>
        {
            entity.HasKey(e => e.ScheduleId);

            entity.ToTable("ScheduleList");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.ForumListId).HasColumnName("ForumListID");
            entity.Property(e => e.TripId).HasColumnName("TripID");

            entity.HasOne(d => d.ForumList).WithMany(p => p.ScheduleLists)
                .HasForeignKey(d => d.ForumListId)
                .HasConstraintName("FK_ScheduleList_ForumList");

            entity.HasOne(d => d.Trip).WithMany(p => p.ScheduleLists)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_ScheduleList_Trip");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Status1)
                .HasMaxLength(50)
                .HasColumnName("Status");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.TripId).HasName("PK_出團");

            entity.ToTable("Trip");

            entity.Property(e => e.TripId).HasColumnName("TripID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.DiscountExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Product).WithMany(p => p.Trips)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Trip_ProductList");
        });

        modelBuilder.Entity<TripDetail>(entity =>
        {
            entity.ToTable("TripDetail");

            entity.Property(e => e.TripDetailId).HasColumnName("TripDetailID");
            entity.Property(e => e.ImagePath).HasMaxLength(50);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.TripDetail1).HasColumnName("TripDetail");

            entity.HasOne(d => d.Product).WithMany(p => p.TripDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_TripDetail_ProductList");
        });

        modelBuilder.Entity<TripDetailPhotoList>(entity =>
        {
            entity.ToTable("TripDetailPhotoList");

            entity.Property(e => e.TripDetailPhotoListId).HasColumnName("TripDetailPhotoListID");
            entity.Property(e => e.ImagePath).HasMaxLength(50);
            entity.Property(e => e.TripDetailId).HasColumnName("TripDetailID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
