using System;
using System.Collections.Generic;

namespace Dragza.Domain.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? BussinesName { get; set; }

    public bool? IsPharmacy { get; set; }

    public string? Photo { get; set; }

    public bool? IsActive { get; set; }

    public string? UserName { get; set; }

    public string? NomalizedUserName { get; set; }

    public string? Email { get; set; }

    public bool? EmailConfirmed { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public bool? PhoneConfirmed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public Guid? RegionId { get; set; }

    public decimal? MinOrder { get; set; }

    public Guid? SubAreaId { get; set; }

    public virtual ICollection<BalanceAccount> BalanceAccounts { get; set; } = new List<BalanceAccount>();

    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();

    public virtual ICollection<Order> OrderInventoryUsers { get; set; } = new List<Order>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Order> OrderPharmacyUsers { get; set; } = new List<Order>();

    public virtual ICollection<PharmacyDetail> PharmacyDetailPurchasingManagerNavigations { get; set; } = new List<PharmacyDetail>();

    public virtual ICollection<PharmacyDetail> PharmacyDetailUsers { get; set; } = new List<PharmacyDetail>();

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

    public virtual Region? Region { get; set; }

    public virtual ICollection<ReturnOrder> ReturnOrderInventoryUsers { get; set; } = new List<ReturnOrder>();

    public virtual ICollection<ReturnOrder> ReturnOrderPharmacyUsers { get; set; } = new List<ReturnOrder>();

    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();

    public virtual Governate? SubArea { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
}
