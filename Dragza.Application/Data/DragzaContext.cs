using System;
using System.Collections.Generic;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Dragza.Application.Data;

public partial class DragzaContext : DbContext
{
    public DragzaContext()
    {
    }

    public DragzaContext(DbContextOptions<DragzaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActiveIngredient> ActiveIngredients { get; set; }

    public virtual DbSet<BalanceAccount> BalanceAccounts { get; set; }

    public virtual DbSet<BalanceTransaction> BalanceTransactions { get; set; }

    public virtual DbSet<BestSellerProduct> BestSellerProducts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<CouponApplicability> CouponApplicabilities { get; set; }

    public virtual DbSet<CouponUsage> CouponUsages { get; set; }

    public virtual DbSet<Destrict> Destricts { get; set; }

    public virtual DbSet<Governate> Governates { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceType> InvoiceTypes { get; set; }

    public virtual DbSet<MainCategory> MainCategories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PharmacyDetail> PharmacyDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductPrice> ProductPrices { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<ReturnOrder> ReturnOrders { get; set; }

    public virtual DbSet<ReturnReason> ReturnReasons { get; set; }

    public virtual DbSet<ReturnedItem> ReturnedItems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<ActiveIngredient>(entity =>
        {
            entity.ToTable("ActiveIngredient");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<BalanceAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BalanceA__3214EC07AEDCCED6");

            entity.HasIndex(e => e.UserId, "IX_BalanceAccounts_UserId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreditLimit).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.BalanceAccounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BalanceAccounts_Users");
        });

        modelBuilder.Entity<BalanceTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BalanceT__3214EC07E9118AE8");

            entity.HasIndex(e => e.BalanceAccountId, "IX_BalanceTransactions_BalanceAccountId");

            entity.HasIndex(e => e.OrderId, "IX_BalanceTransactions_OrderId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.BalanceAccount).WithMany(p => p.BalanceTransactions)
                .HasForeignKey(d => d.BalanceAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BalanceTransactions_BalanceAccounts");

            entity.HasOne(d => d.Order).WithMany(p => p.BalanceTransactions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_BalanceTransactions_Orders");

            entity.HasOne(d => d.RelatedTransaction).WithMany(p => p.InverseRelatedTransaction)
                .HasForeignKey(d => d.RelatedTransactionId)
                .HasConstraintName("FK_BalanceTransactions_RelatedTransactions");
        });

        modelBuilder.Entity<BestSellerProduct>(entity =>
        {
            entity.ToTable("BestSellerProduct");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Product).WithMany(p => p.BestSellerProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BestSellerProduct_Product");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Pref).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.MainCategory).WithMany(p => p.Categories)
                .HasForeignKey(d => d.MainCategoryId)
                .HasConstraintName("FK_Category_MainCategory");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Area");

            entity.ToTable("City");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Governate).WithMany(p => p.Cities)
                .HasForeignKey(d => d.GovernateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_Governate");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__coupons__3213E83FC3864A76");

            entity.ToTable("coupons");

            entity.HasIndex(e => new { e.IsActive, e.StartDate, e.EndDate }, "IX_coupons_active");

            entity.HasIndex(e => e.Code, "IX_coupons_code");

            entity.HasIndex(e => e.Code, "UQ__coupons__357D4CF970C4BAC3").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(20)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discount_value");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaximumDiscountAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("maximum_discount_amount");
            entity.Property(e => e.MinimumOrderAmount)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("minimum_order_amount");
            entity.Property(e => e.PerUserLimit)
                .HasDefaultValue(1)
                .HasColumnName("per_user_limit");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
        });

        modelBuilder.Entity<CouponApplicability>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__coupon_a__3213E83FECA136AB");

            entity.ToTable("coupon_applicability");

            entity.HasIndex(e => new { e.CouponId, e.ApplicableType, e.ApplicableId }, "IX_coupon_applicability");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ApplicableId).HasColumnName("applicable_id");
            entity.Property(e => e.ApplicableType)
                .HasMaxLength(20)
                .HasColumnName("applicable_type");
            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Coupon).WithMany(p => p.CouponApplicabilities)
                .HasForeignKey(d => d.CouponId)
                .HasConstraintName("FK__coupon_ap__coupo__6166761E");
        });

        modelBuilder.Entity<CouponUsage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__coupon_u__3213E83F8C342324");

            entity.ToTable("coupon_usages");

            entity.HasIndex(e => e.CouponId, "IX_coupon_usages_coupon");

            entity.HasIndex(e => e.OrderId, "IX_coupon_usages_order");

            entity.HasIndex(e => e.UserId, "IX_coupon_usages_user");

            entity.HasIndex(e => new { e.CouponId, e.OrderId }, "UQ_CouponOrder").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CouponId).HasColumnName("coupon_id");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.UsedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Coupon).WithMany(p => p.CouponUsages)
                .HasForeignKey(d => d.CouponId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__coupon_us__coupo__690797E6");

            entity.HasOne(d => d.Order).WithMany(p => p.CouponUsages)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_coupon_usages_Order");

            entity.HasOne(d => d.User).WithMany(p => p.CouponUsages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_coupon_usages_User");
        });

        modelBuilder.Entity<Destrict>(entity =>
        {
            entity.ToTable("Destrict");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.City).WithMany(p => p.Destricts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Destrict_City");
        });

        modelBuilder.Entity<Governate>(entity =>
        {
            entity.ToTable("Governate");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Region).WithMany(p => p.Governates)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Governate_Region");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoice");

            entity.HasIndex(e => e.OrderId, "IX_Invoices_OrderId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.InvoiceType).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.InvoiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_InvoiceTypes");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Order");
        });

        modelBuilder.Entity<InvoiceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InvoiceT__3214EC07A00F4181");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<MainCategory>(entity =>
        {
            entity.ToTable("MainCategory");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.HasIndex(e => e.PharmacyUserId, "IX_Orders_PharmacyUserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ApprovalDate).HasColumnType("datetime");
            entity.Property(e => e.CashPaid)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreditUsed)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DeliverDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.CreditAccount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CreditAccountId)
                .HasConstraintName("FK_Order_BalanceAccounts");

            entity.HasOne(d => d.InventoryUser).WithMany(p => p.OrderInventoryUsers)
                .HasForeignKey(d => d.InventoryUserId)
                .HasConstraintName("FK_Order_User1");

            entity.HasOne(d => d.PharmacyUser).WithMany(p => p.OrderPharmacyUsers)
                .HasForeignKey(d => d.PharmacyUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Product");

            entity.HasOne(d => d.ProductPrice).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductPriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_ProductPrice");
        });

        modelBuilder.Entity<PharmacyDetail>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ArabicName).HasMaxLength(50);
            entity.Property(e => e.CommercialRegisteryAttach).HasMaxLength(50);
            entity.Property(e => e.CommercialRegisteryNumber).HasMaxLength(50);
            entity.Property(e => e.DemoAgentCode).HasMaxLength(50);
            entity.Property(e => e.EnglishName).HasMaxLength(50);
            entity.Property(e => e.LandLineNumber).HasMaxLength(50);
            entity.Property(e => e.NationalId).HasMaxLength(50);
            entity.Property(e => e.NationalIdAttach).HasMaxLength(50);
            entity.Property(e => e.OwnersgipAttach).HasMaxLength(50);
            entity.Property(e => e.PharmacyLicenseAttach).HasMaxLength(50);
            entity.Property(e => e.PharmacyLicenseNo).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.TaxationCardAttach).HasMaxLength(50);
            entity.Property(e => e.TaxationCardNo).HasMaxLength(50);

            entity.HasOne(d => d.PurchasingManagerNavigation).WithMany(p => p.PharmacyDetailPurchasingManagerNavigations)
                .HasForeignKey(d => d.PurchasingManager)
                .HasConstraintName("FK_PharmacyDetails_User1");

            entity.HasOne(d => d.User).WithMany(p => p.PharmacyDetailUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PharmacyDetails_User");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(1);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Preef).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ActiveIngerdient).WithMany(p => p.Products)
                .HasForeignKey(d => d.ActiveIngerdientId)
                .HasConstraintName("FK_Product_ActiveIngredient");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");

            entity.HasOne(d => d.MainCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.MainCategoryId)
                .HasConstraintName("FK_Product_MainCategory");
		
		});

        modelBuilder.Entity<ProductPrice>(entity =>
        {
            entity.ToTable("ProductPrice");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalesPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.ProductPrices)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPrice_Category");

            entity.HasOne(d => d.InventoryUser).WithMany(p => p.ProductPrices)
                .HasForeignKey(d => d.InventoryUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPrice_User");

            entity.HasOne(d => d.MainCategory).WithMany(p => p.ProductPrices)
                .HasForeignKey(d => d.MainCategoryId)
                .HasConstraintName("FK_ProductPrice_MainCategory");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPrices)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductPrice_Product");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.ToTable("Region");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Lang).HasMaxLength(50);
            entity.Property(e => e.Lat)
                .HasMaxLength(50)
                .HasColumnName("lat");
            entity.Property(e => e.RegionName).HasMaxLength(50);
        });

        modelBuilder.Entity<ReturnOrder>(entity =>
        {
            entity.ToTable("ReturnOrder");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ApprovalDate).HasColumnType("datetime");
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.TotalReturnValue).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.InventoryUser).WithMany(p => p.ReturnOrderInventoryUsers)
                .HasForeignKey(d => d.InventoryUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnOrder_User1");

            entity.HasOne(d => d.Order).WithMany(p => p.ReturnOrders)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnOrder_Order");

            entity.HasOne(d => d.PharmacyUser).WithMany(p => p.ReturnOrderPharmacyUsers)
                .HasForeignKey(d => d.PharmacyUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnOrder_User");
        });

        modelBuilder.Entity<ReturnReason>(entity =>
        {
            entity.ToTable("ReturnReason");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Reason).HasMaxLength(50);
        });

        modelBuilder.Entity<ReturnedItem>(entity =>
        {
            entity.ToTable("ReturnedItem");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Order).WithMany(p => p.ReturnedItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnedItem_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.ReturnedItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnedItem_Product");

            entity.HasOne(d => d.ProductPrice).WithMany(p => p.ReturnedItems)
                .HasForeignKey(d => d.ProductPriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnedItem_ProductPrice");

            entity.HasOne(d => d.Reason).WithMany(p => p.ReturnedItems)
                .HasForeignKey(d => d.ReasonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnedItem_ReturnReason");

            entity.HasOne(d => d.ReturnOrder).WithMany(p => p.ReturnedItems)
                .HasForeignKey(d => d.ReturnOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnedItem_ReturnOrder");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BussinesName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.MinOrder).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NomalizedUserName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Region).WithMany(p => p.Users)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_User_Region");

            entity.HasOne(d => d.SubArea).WithMany(p => p.Users)
                .HasForeignKey(d => d.SubAreaId)
                .HasConstraintName("FK_User_Governate");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_User");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.ToTable("UserToken");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreationDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserToken_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
