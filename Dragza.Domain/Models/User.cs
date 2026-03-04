//using System;
//using System.Collections.Generic;

//namespace Dragza.Domain.Models;

//public partial class User
//{
//    public Guid Id { get; set; }

//    public string FirstName { get; set; } = string.Empty;
//    public string LastName { get; set; } = string.Empty;
//    public string UserName {  get; set; } = string.Empty;    
//    public string? Email { get; set; }
//    public string? Password { get; set; }
//    public string? PhoneNumber { get; set; }
//    public DateTime? CreatedAt { get; set; }
//    public DateTime? UpdatedAt { get; set; }
//    public bool IsActive { get; set; }
//    public bool? IsDeleted { get; set; }

//    public DateTime? DeletedDate { get; set; }



//    public decimal? MinOrder { get; set; }

//    public virtual ICollection<BalanceAccount> BalanceAccounts { get; set; } = new List<BalanceAccount>();

//    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();

//    public virtual ICollection<Order> OrderInventoryUsers { get; set; } = new List<Order>();

//    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

//    //public virtual ICollection<Order> OrderPharmacyUsers { get; set; } = new List<Order>();

//    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

//    public virtual ICollection<ReturnOrder> ReturnOrderInventoryUsers { get; set; } = new List<ReturnOrder>();

//    public virtual ICollection<ReturnOrder> ReturnOrderPharmacyUsers { get; set; } = new List<ReturnOrder>();

//    public virtual ICollection<ReturnedItem> ReturnedItems { get; set; } = new List<ReturnedItem>();

//    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
//    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
//}



using Dragza.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<BalanceAccount> BalanceAccounts { get; set; } = new List<BalanceAccount>();
}
