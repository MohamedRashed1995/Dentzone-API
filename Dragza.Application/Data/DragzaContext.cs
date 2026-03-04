    //using System;
    //using System.Collections.Generic;
    //using Dragza.Domain.Models;
    //using Microsoft.EntityFrameworkCore;

    //namespace Dragza.Application.Data;

    //public partial class DragzaContext : DbContext
    //{
    //    public DragzaContext() { }

    //    public DragzaContext(DbContextOptions<DragzaContext> options)
    //        : base(options) { }

    //    public virtual DbSet<BalanceAccount> BalanceAccounts { get; set; }
    //    public virtual DbSet<BalanceTransaction> BalanceTransactions { get; set; }
    //    public virtual DbSet<BestSellerProduct> BestSellerProducts { get; set; }
    //    public virtual DbSet<Category> Categories { get; set; }
    //    public virtual DbSet<Coupon> Coupons { get; set; }
    //    public virtual DbSet<CouponApplicability> CouponApplicabilities { get; set; }
    //    public virtual DbSet<CouponUsage> CouponUsages { get; set; }
    //    public virtual DbSet<Invoice> Invoices { get; set; }
    //    public virtual DbSet<InvoiceType> InvoiceTypes { get; set; }
    //    public virtual DbSet<Order> Orders { get; set; }
    //    public virtual DbSet<OrderItem> OrderItems { get; set; }
    //    public virtual DbSet<Product> Products { get; set; }
    //    public virtual DbSet<ProductPrice> ProductPrices { get; set; }
    //    public virtual DbSet<ReturnOrder> ReturnOrders { get; set; }
    //    public virtual DbSet<ReturnReason> ReturnReasons { get; set; }
    //    public virtual DbSet<ReturnedItem> ReturnedItems { get; set; }
    //    public virtual DbSet<Role> Roles { get; set; }
    //    public virtual DbSet<User> Users { get; set; }
    //    public virtual DbSet<UserRole> UserRoles { get; set; }
    //    public virtual DbSet<UserToken> UserTokens { get; set; }
    //    public virtual DbSet<Cart> Carts { get; set; }
    //    public virtual DbSet<CartItem> CartItems { get; set; }
    //    public virtual DbSet<Notifacation> Notifacations { get; set; }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.UseCollation("Latin1_General_CI_AS");

    //        #region BalanceAccount
    //        modelBuilder.Entity<BalanceAccount>(entity =>
    //        {
    //            entity.HasKey(e => e.Id);
    //            entity.HasIndex(e => e.UserId, "IX_BalanceAccounts_UserId");
    //            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
    //            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
    //            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
    //            entity.Property(e => e.CreditLimit).HasColumnType("decimal(18, 2)");
    //        });
    //        #endregion

    //        #region BalanceTransaction
    //        modelBuilder.Entity<BalanceTransaction>(entity =>
    //        {
    //            entity.HasKey(e => e.Id);
    //            entity.HasIndex(e => e.BalanceAccountId, "IX_BalanceTransactions_BalanceAccountId");
    //            entity.HasIndex(e => e.OrderId, "IX_BalanceTransactions_OrderId");
    //            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
    //            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
    //            entity.Property(e => e.Description).HasMaxLength(500);
    //            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(sysutcdatetime())");

    //            entity.HasOne(d => d.BalanceAccount).WithMany(p => p.BalanceTransactions)
    //                .HasForeignKey(d => d.BalanceAccountId)
    //                .OnDelete(DeleteBehavior.ClientSetNull);

    //            entity.HasOne(d => d.Order).WithMany(p => p.BalanceTransactions)
    //                .HasForeignKey(d => d.OrderId)
    //                .OnDelete(DeleteBehavior.ClientSetNull);

    //            entity.HasOne(d => d.RelatedTransaction).WithMany(p => p.InverseRelatedTransaction)
    //                .HasForeignKey(d => d.RelatedTransactionId);
    //        });
    //        #endregion

    //        #region Order
    //        modelBuilder.Entity<Order>(entity =>
    //        {
    //            entity.ToTable("Order");

    //            entity.Property(e => e.Id).ValueGeneratedNever();
    //            entity.Property(e => e.OrderDate).HasColumnType("datetime");
    //            entity.Property(e => e.ApprovalDate).HasColumnType("datetime");
    //            entity.Property(e => e.DeliverDate).HasColumnType("datetime");
    //            entity.Property(e => e.OrderNumber).HasMaxLength(50);
    //            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");
    //            entity.Property(e => e.CreditUsed).HasColumnType("decimal(18, 2)").HasDefaultValue(0m);
    //            entity.Property(e => e.CashPaid).HasColumnType("decimal(18, 2)").HasDefaultValue(0m);

    //            entity.HasOne(d => d.User)
    //                  .WithMany(u => u.Orders)
    //                  .HasForeignKey(d => d.UserId)
    //                  .OnDelete(DeleteBehavior.ClientSetNull)
    //                  .HasConstraintName("FK_Order_User");

    //            entity.HasOne(d => d.CreditAccount)
    //                  .WithMany(p => p.Orders)
    //                  .HasForeignKey(d => d.CreditAccountId)
    //                  .OnDelete(DeleteBehavior.NoAction) // تم تعديلها لمنع Multiple Cascade Paths
    //                  .HasConstraintName("FK_Order_BalanceAccounts");
    //        });
    //        #endregion

    //        #region OrderItem
    //        modelBuilder.Entity<OrderItem>(entity =>
    //        {
    //            entity.ToTable("OrderItem");
    //            entity.Property(e => e.Id).ValueGeneratedNever();
    //            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

    //            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
    //                .HasForeignKey(d => d.OrderId)
    //                .OnDelete(DeleteBehavior.NoAction); // تعديل هنا

    //            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
    //                .HasForeignKey(d => d.ProductId)
    //                .OnDelete(DeleteBehavior.NoAction); // تعديل هنا

    //            entity.HasOne(d => d.ProductPrice).WithMany(p => p.OrderItems)
    //                .HasForeignKey(d => d.ProductPriceId)
    //                .OnDelete(DeleteBehavior.NoAction); // تعديل هنا
    //        });
    //        #endregion


    //        #region ReturnedItem
    //        modelBuilder.Entity<ReturnedItem>(entity =>
    //        {
    //            entity.HasKey(e => e.Id);

    //            entity.HasOne(d => d.Product)
    //                  .WithMany()
    //                  .HasForeignKey(d => d.ProductId)
    //                  .OnDelete(DeleteBehavior.NoAction); // منع cascade

    //            entity.HasOne(d => d.Order)
    //                  .WithMany(p => p.ReturnedItems)
    //                  .HasForeignKey(d => d.OrderId)
    //                  .OnDelete(DeleteBehavior.NoAction); // منع cascade

    //            entity.HasOne(d => d.ReturnOrder)
    //                  .WithMany(p => p.ReturnedItems)
    //                  .HasForeignKey(d => d.ReturnOrderId)
    //                  .OnDelete(DeleteBehavior.NoAction); // منع cascade

    //            entity.HasOne(d => d.ProductPrice)
    //                  .WithMany()
    //                  .HasForeignKey(d => d.ProductPriceId)
    //                  .OnDelete(DeleteBehavior.NoAction); // منع cascade

    //            entity.HasOne(d => d.Inventory)
    //                  .WithMany()
    //                  .HasForeignKey(d => d.InventoryId)
    //                  .OnDelete(DeleteBehavior.NoAction); // لو موجود
    //        });
    //        #endregion


    //        #region User
    //        modelBuilder.Entity<User>(entity =>
    //        {
    //            entity.ToTable("User");

    //            entity.Property(e => e.Id).ValueGeneratedNever();
    //            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
    //            entity.Property(e => e.Email).HasMaxLength(50);
    //            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
    //        });
    //        #endregion

    //        #region UserRole
    //        modelBuilder.Entity<UserRole>(entity =>
    //        {
    //            entity.Property(e => e.Id).ValueGeneratedNever();

    //            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
    //                .HasForeignKey(d => d.RoleId)
    //                .OnDelete(DeleteBehavior.NoAction); // تعديل هنا

    //            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
    //                .HasForeignKey(d => d.UserId)
    //                .OnDelete(DeleteBehavior.NoAction); // تعديل هنا
    //        });
    //        #endregion

    //        #region UserToken
    //        modelBuilder.Entity<UserToken>(entity =>
    //        {
    //            entity.ToTable("UserToken");
    //            entity.Property(e => e.Id).ValueGeneratedNever();
    //            entity.Property(e => e.CreationDate).HasColumnType("datetime");

    //            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
    //                .HasForeignKey(d => d.UserId)
    //                .OnDelete(DeleteBehavior.NoAction); // تعديل هنا
    //        });
    //        #endregion

    //        #region CartItem
    //        modelBuilder.Entity<CartItem>(entity =>
    //        {
    //            entity.HasKey(e => e.Id);

    //            entity.HasOne(d => d.Cart)
    //                  .WithMany(c => c.Items)
    //                  .HasForeignKey(d => d.CartId)
    //                  .OnDelete(DeleteBehavior.NoAction);

    //            entity.HasOne(d => d.Product)
    //                  .WithMany()
    //                  .HasForeignKey(d => d.ProductId)
    //                  .OnDelete(DeleteBehavior.NoAction);

    //            entity.HasOne(d => d.ProductPrice)
    //                  .WithMany()
    //                  .HasForeignKey(d => d.ProductPriceId)
    //                  .OnDelete(DeleteBehavior.NoAction);

    //            entity.HasOne(d => d.InventoryUser)
    //                  .WithMany()
    //                  .HasForeignKey(d => d.InventoryUserId)
    //                  .OnDelete(DeleteBehavior.NoAction);
    //        });
    //        #endregion

    //        #region Seed Roles

    //        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    //        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    //        var sellerRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    //        modelBuilder.Entity<Role>().HasData(
    //            new Role { Id = adminRoleId, Name = "Admin" },
    //            new Role { Id = userRoleId, Name = "User" },
    //            new Role { Id = sellerRoleId, Name = "Seller" }
    //        );

    //        #endregion

    //        OnModelCreatingPartial(modelBuilder);
    //    }

    //    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    //}


    using System;
    using System.Collections.Generic;
    using Dragza.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity; // للـ PasswordHasher

    namespace Dragza.Application.Data;

    public partial class DragzaContext : DbContext
    {
    private const string AdminPasswordPlain = "AdminPassword123!";
    public DragzaContext() { }

        public DragzaContext(DbContextOptions<DragzaContext> options)
            : base(options) { }

        public virtual DbSet<BalanceAccount> BalanceAccounts { get; set; }
        public virtual DbSet<BalanceTransaction> BalanceTransactions { get; set; }
        public virtual DbSet<BestSellerProduct> BestSellerProducts { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<CouponApplicability> CouponApplicabilities { get; set; }
        public virtual DbSet<CouponUsage> CouponUsages { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceType> InvoiceTypes { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductPrice> ProductPrices { get; set; }
        public virtual DbSet<ReturnOrder> ReturnOrders { get; set; }
        public virtual DbSet<ReturnReason> ReturnReasons { get; set; }
        public virtual DbSet<ReturnedItem> ReturnedItems { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserToken> UserTokens { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Notifacation> Notifacations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            // ... كل Entity configurations زي ما هما عندك (BalanceAccount, Order, UserRole, إلخ)
            #region BalanceAccount
            modelBuilder.Entity<BalanceAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId, "IX_BalanceAccounts_UserId");
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.CreditLimit).HasColumnType("decimal(18, 2)");
            });
            #endregion

            #region BalanceTransaction
            modelBuilder.Entity<BalanceTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.BalanceAccountId, "IX_BalanceTransactions_BalanceAccountId");
                entity.HasIndex(e => e.OrderId, "IX_BalanceTransactions_OrderId");
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.TransactionDate).HasDefaultValueSql("(sysutcdatetime())");

                entity.HasOne(d => d.BalanceAccount).WithMany(p => p.BalanceTransactions)
                    .HasForeignKey(d => d.BalanceAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Order).WithMany(p => p.BalanceTransactions)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RelatedTransaction).WithMany(p => p.InverseRelatedTransaction)
                    .HasForeignKey(d => d.RelatedTransactionId);
            });
            #endregion

            #region Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.ApprovalDate).HasColumnType("datetime");
                entity.Property(e => e.DeliverDate).HasColumnType("datetime");
                entity.Property(e => e.OrderNumber).HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");
                entity.Property(e => e.CreditUsed).HasColumnType("decimal(18, 2)").HasDefaultValue(0m);
                entity.Property(e => e.CashPaid).HasColumnType("decimal(18, 2)").HasDefaultValue(0m);

                entity.HasOne(d => d.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Order_User");

                entity.HasOne(d => d.CreditAccount)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(d => d.CreditAccountId)
                      .OnDelete(DeleteBehavior.NoAction) // تم تعديلها لمنع Multiple Cascade Paths
                      .HasConstraintName("FK_Order_BalanceAccounts");
            });
            #endregion

            #region OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItem");
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.NoAction); // تعديل هنا

                entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.NoAction); // تعديل هنا

                entity.HasOne(d => d.ProductPrice).WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductPriceId)
                    .OnDelete(DeleteBehavior.NoAction); // تعديل هنا
            });
            #endregion


            #region ReturnedItem
            modelBuilder.Entity<ReturnedItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Product)
                      .WithMany()
                      .HasForeignKey(d => d.ProductId)
                      .OnDelete(DeleteBehavior.NoAction); // منع cascade

                entity.HasOne(d => d.Order)
                      .WithMany(p => p.ReturnedItems)
                      .HasForeignKey(d => d.OrderId)
                      .OnDelete(DeleteBehavior.NoAction); // منع cascade

                entity.HasOne(d => d.ReturnOrder)
                      .WithMany(p => p.ReturnedItems)
                      .HasForeignKey(d => d.ReturnOrderId)
                      .OnDelete(DeleteBehavior.NoAction); // منع cascade

                entity.HasOne(d => d.ProductPrice)
                      .WithMany()
                      .HasForeignKey(d => d.ProductPriceId)
                      .OnDelete(DeleteBehavior.NoAction); // منع cascade

                entity.HasOne(d => d.Inventory)
                      .WithMany()
                      .HasForeignKey(d => d.InventoryId)
                      .OnDelete(DeleteBehavior.NoAction); // لو موجود
            });
            #endregion


            #region User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            });
            #endregion

            #region UserRole
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.NoAction); // تعديل هنا

                entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction); // تعديل هنا
            });
            #endregion

            #region UserToken
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("UserToken");
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction); // تعديل هنا
            });
        #endregion
        
        
        
        #region Addresses
        modelBuilder.Entity<Address>()
            .HasOne(a => a.User)
            .WithMany(b => b.Addresses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion
        
        
        
        #region CartItem
        modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Cart)
                      .WithMany(c => c.Items)
                      .HasForeignKey(d => d.CartId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Product)
                      .WithMany()
                      .HasForeignKey(d => d.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.ProductPrice)
                      .WithMany()
                      .HasForeignKey(d => d.ProductPriceId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.InventoryUser)
                      .WithMany()
                      .HasForeignKey(d => d.InventoryUserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
            #endregion

        
            #region Seed Roles
            var adminRoleId = Guid.Parse("8C2F4F3A-7F6D-4DB8-8B02-4A04D31F35D6");
            var userRoleId = Guid.Parse("E48E5A9F-2074-4DE9-A849-5C69FDD45E4E");
            var InventoryManagerRoleId = Guid.Parse("1A5A84FB-23C3-4F9B-A122-4C5BC6C5CB2D");

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, Name = "Admin" },
                new Role { Id = userRoleId, Name = "User" },
                new Role { Id = InventoryManagerRoleId, Name = "Inventory" }
            );
            #endregion

        //    #region Seed Admin User
        //    var adminUserId = Guid.NewGuid();

        //    var passwordHasher = new PasswordHasher<User>();
        //    var adminUser = new User
        //    {
        //        Id = adminUserId,
        //        FullName = "Admin User",
        //        Email = "admin@dentzone.com",
        //        PhoneNumber = "01234567890",
        //        Password = "Password123!",
        //        //Password = passwordHasher.HashPassword(adminUser,"Password123!"),
        //        IsActive = true,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    var hasher = new PasswordHasher<User>();
        //    adminUser.Password = hasher.HashPassword(adminUser, AdminPasswordPlain);

        //    modelBuilder.Entity<User>().HasData(adminUser);

        //    modelBuilder.Entity<UserRole>().HasData(
        //        new UserRole
        //        {
        //            Id = Guid.NewGuid(),
        //            UserId = adminUserId,
        //            RoleId = adminRoleId
        //        }
        //    );
        //#endregion

        OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
