using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.DTO.Order;
using Dragza.Domain.DTO.ReturnOrder;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;

namespace Dragza.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<User, CreateUserDto>();

            CreateMap<User, UserResponseDto>();
            //.ForMember(dest=>dest.PharmacyDetails,opt=>opt.MapFrom(src=>src.PharmacyDetailUsers.FirstOrDefault()));

            //CreateMap<PharmacyDetail, PharmacyDetailsDto>();


            CreateMap<User, UserWithSpecificRolesDto>();
            CreateMap<UserWithSpecificRolesDto, User>();

            CreateMap<User, UserWithSpecificRolesDto>()
                .ForMember(dest => dest.AddressLines,
                    opt => opt.MapFrom(src =>
                        src.Addresses != null && src.Addresses.Any()
                            ? string.Join(" - ", src.Addresses.Select(a => a.AddressLine))
                            : string.Empty));


            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ImageName, opt =>
                    opt.MapFrom((src, dest, destMember, context) =>
                        string.IsNullOrEmpty(src.ImageName)
                            ? null
                            : (context.Items["BaseCategoryUrl"]?.ToString() ?? "") + src.ImageName
            )); 
            
            CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                ;

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Product, ProductAddDto>().ReverseMap();
           
            CreateMap<CreateProductDto, Product>();
            CreateMap<CartDto, Cart>().ReverseMap();
            CreateMap<CartItemDto, CartItem>().ReverseMap();
            CreateMap<ProductPriceDto, ProductPrice>().ReverseMap();
            CreateMap<UserDtoCart, User>().ReverseMap();
            CreateMap<ProductDtoCart, Product>().ReverseMap();



            // ProductPrice mappings
            CreateMap<ProductPrice, ProductPriceResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductArabicName, opt => opt.MapFrom(src => src.Product.ArabicName))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OrderStatus)src.Status))
            
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                //.ForMember(dest => dest.InventoryName, opt => opt.MapFrom(src => src.ProductPrice.InventoryUser.UserName))
                .ForMember(dest => dest.InventoryUserId, opt => opt.MapFrom(src => src.Order.InventoryUserId))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.ProductPrice.SalesPrice-(src.ProductPrice.SalesPrice*src.ProductPrice.DiscountRate/100)));
            CreateMap<OrderItemDto, OrderItem>();

            CreateMap<ReturnOrder, ReturnOrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ReturnOrderStatus)src.Statuse))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ReturnedItems));

            CreateMap<ReturnedItem, ReturnedItemDto>()
                .ForMember(dest => dest.ReasonId, opt => opt.MapFrom(src => src.Reason.Id));

            CreateMap<ReturnReason, ReturnReasonDto>();
            CreateMap<CreateReturnReasonDto, ReturnReason>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));
            CreateMap<UpdateReturnReasonDto, ReturnReason>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));
    //.ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders))
    //.ForMember(dest => dest.BalanceAccounts, opt => opt.MapFrom(src => src.BalanceAccounts));

            // لو عايز تعمل العكس (DTO -> Entity) ممكن تعمل حاجة زي:
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore()) // لأن الـ Roles في DTO عبارة عن أسماء بس
                .ForMember(dest => dest.Orders, opt => opt.Ignore())     // هتتعامل مع الـ Orders في Service مش من DTO
                .ForMember(dest => dest.BalanceAccounts, opt => opt.Ignore());

            //  .ForMember(dest => dest.RegionId, opt => opt.MapFrom(src => src.MinOrder ?? 0m));
           

    

            //CreateMap<ProductPrice, ProductPriceResponseDto>()
            //    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            //    .ForMember(dest => dest.ProductArabicName, opt => opt.MapFrom(src => src.Product.ArabicName));

            CreateMap<ProductPrice, ProductPriceResponseDto>()
    // ProductPrice specific fields
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PurchasePrice))
                .ForMember(dest => dest.SalesPrice, opt => opt.MapFrom(src => src.SalesPrice))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate ?? DateTime.Now))
                .ForMember(dest => dest.InventoryUserId, opt => opt.MapFrom(src => src.InventoryUserId))
                //.ForMember(dest => dest.InventoryUserName, opt => opt.MapFrom(src => src.Inventory.FullName))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))

    // Product details from navigation property
    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
    .ForMember(dest => dest.ProductArabicName, opt => opt.MapFrom(src => src.Product.ArabicName))
    .ForMember(dest => dest.Preef, opt => opt.MapFrom(src => src.Product.Preef))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product.Description))
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Product.CreatedAt))
    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Product.UpdatedAt))
    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Product.Image))
    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Product.Category));
	//.ForMember(dest => dest.ActiveIngredient, opt => opt.MapFrom(src => src.Product.ActiveIngerdient));

	//		CreateMap<Product, ProductPriceResponseDto>()
	//.ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
	//.ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
	//.ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
	//.ForMember(dest => dest.ProductArabicName, opt => opt.MapFrom(src => src.ArabicName))
	//.ForMember(dest => dest.Preef, opt => opt.MapFrom(src => src.Preef))
	//.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
	//.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
	//.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
	//.ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
	//.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
	//.ForMember(dest => dest.ActiveIngredient, opt => opt.MapFrom(src => src.ActiveIngerdient))
	//.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
	//.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
	//.ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src =>
	//	src.ProductPrices.Any() ?
	//	src.ProductPrices.FirstOrDefault().PurchasePrice :
	//	0m)) // Default to 0 since Product doesn't have PurchasePrice
	//.ForMember(dest => dest.SalesPrice, opt => opt.MapFrom(src =>
	//	src.ProductPrices.Any() ?
	//	src.ProductPrices.FirstOrDefault().SalesPrice :
	//	0m)) // Default to 0 since Product doesn't have SalesPrice
	//.ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src =>
	//	src.UpdatedAt ?? src.CreatedAt ?? DateTime.Now))
	//.ForMember(dest => dest.InventoryUserId, opt => opt.MapFrom(src =>
	//	src.ProductPrices.Any() && src.ProductPrices.FirstOrDefault().InventoryUser != null ?
	//	src.ProductPrices.FirstOrDefault().InventoryUser.Id :
	//	Guid.Empty)) // Default to Empty since Product doesn't have InventoryUserId
	//.ForMember(dest => dest.InventoryUserName, opt => opt.MapFrom(src =>
	//	src.ProductPrices.Any() && src.ProductPrices.FirstOrDefault().InventoryUser != null ?
	//	src.ProductPrices.FirstOrDefault().InventoryUser.BussinesName :
	//	"")) // Default to empty since Product doesn't have InventoryUser
	//.ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src =>
	//	src.ProductPrices.Any() ?
	//	src.ProductPrices.FirstOrDefault().StockQuantity :
	//	0));

			//CreateMap<ActiveIngredient, ActiveIngredientDto>();

            CreateMap<ProductPrice, ProductBestPriceDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductArabicName, opt => opt.MapFrom(src => src.Product.ArabicName))
            .ForMember(dest => dest.BestSalesPrice, opt => opt.MapFrom(src => src.SalesPrice))
            .ForMember(dest => dest.PriceId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PriceDate, opt => opt.MapFrom(src => src.CreationDate))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.DiscountRate))
            .ForMember(dest=>dest.Quantity,op=>op.MapFrom(src=>src.StockQuantity)).ReverseMap();



            CreateMap<Product, ProductBestPriceDto>()
          .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
          .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
          .ForMember(dest => dest.ProductArabicName, opt => opt.MapFrom(src => src.ArabicName))
          .ForMember(dest => dest.BestSalesPrice, opt => opt.MapFrom(src => src.ProductPrices.FirstOrDefault().SalesPrice))
          .ForMember(dest => dest.PriceId, opt => opt.MapFrom(src => src.ProductPrices))
       
          .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.ProductPrices.FirstOrDefault().DiscountRate))
          .ForMember(dest => dest.Quantity, op => op.MapFrom(src => src.ProductPrices.FirstOrDefault().StockQuantity)).ReverseMap();



            CreateMap<Product, ProductsDto>()
                .ForMember(des => des.EnName, opt => opt.MapFrom(src => src.Name))
                .ForMember(des=>des.ArName,opt=>opt.MapFrom(src=>src.ArabicName)).ReverseMap();



            CreateMap<CreateProductPriceDto, ProductPrice>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryUserId, opt => opt.Ignore());

            // CreateMap<User, UserWithPharmacyDto>()
            //.ForMember(dest => dest.RegionId, opt => opt.MapFrom(src => src.RegionId))
            //.ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.Region.RegionName))
            //.ForMember(dest => dest.PharmacyDetails, opt => opt.MapFrom(src =>
            // src.IsPharmacy == true ? src.PharmacyDetailUsers.FirstOrDefault() : null));


            // CreateMap<User, UserWithPharmacyDto>()
            //.ForMember(dest => dest.RegionId, opt => opt.MapFrom(src => src.RegionId))
            //.ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.Region == null ? string.Empty : src.Region.RegionName))
            //.ForMember(dest => dest.RegionId, opt => opt.MapFrom(src => src.RegionId))
            //.ForMember(dest => dest.SubAreaId, opt => opt.MapFrom(src => src.SubAreaId))
            //.ForMember(dest => dest.SubAreaName, opt => opt.MapFrom(src => src.SubArea == null ? string.Empty : src.SubArea.Name));
            //.ForMember(dest => dest.PharmacyDetails, opt => opt.MapFrom(src =>
            //    src.IsPharmacy == true
            //    ? src.PharmacyDetailUsers.FirstOrDefault()
            //    : null));

            // PharmacyDetail to PharmacyDetailsDto mapping
            //CreateMap<PharmacyDetail, PharmacyDetailsResponseDto>()
            //    .ForMember(dest => dest.CommercialRegisteryAttachPath,
            //        opt => opt.MapFrom(src => src.CommercialRegisteryAttach))
            //    .ForMember(dest => dest.NationalIdAttachPath,
            //        opt => opt.MapFrom(src => src.NationalIdAttach))
            //    .ForMember(dest => dest.PharmacyLicenseAttachPath,
            //        opt => opt.MapFrom(src => src.PharmacyLicenseAttach))
            //    .ForMember(dest => dest.OwnersgipAttachPath,
            //        opt => opt.MapFrom(src => src.OwnersgipAttach))
            //    .ForMember(dest => dest.TaxationCardAttachPath,
            //        opt => opt.MapFrom(src => src.TaxationCardAttach));

            CreateMap<ProductPrice, ProductPriceDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : ""))
                .ForMember(dest => dest.ImageName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.Product != null && !string.IsNullOrEmpty(src.Product.Image)
                        ? (context.Items["BaseUrl"]?.ToString() ?? "") + src.Product.Image
                        : null))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product != null ? src.Product.Description : ""))
                .ForMember(dest => dest.Preef, opt => opt.MapFrom(src => src.Product != null ? src.Product.Preef : ""))
                .ForMember(dest => dest.ArabicDescription, opt => opt.MapFrom(src => src.Product != null ? src.Product.ArabicDescription : ""))
                .ForMember(dest => dest.ArabicPreef, opt => opt.MapFrom(src => src.Product != null ? src.Product.ArabicPreef : ""))
                .ForMember(dest => dest.Category, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (src.Category == null) return null;

                    var categoryDto = new CategoryDto
                    {
                        Id = src.Category.Id,
                        Name = src.Category.Name,
                        ArabicName = src.Category.ArabicName,
                        Pref = src.Category.Pref,
                        Description = src.Category.Description,
                        ImageName = string.IsNullOrEmpty(src.Category.ImageName)
                            ? null
                            : (context.Items["BaseCategoryUrl"]?.ToString() ?? "") + src.Category.ImageName
                    };

                    return categoryDto;
                }))
                .ForMember(dest => dest.InventoryUser, opt => opt.MapFrom(src => src.Inventory != null ? src.Inventory : null))
                .ForMember(dest => dest.ProductPriceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PurchasePrice))
                .ForMember(dest => dest.SalesPrice, opt => opt.MapFrom(src => src.SalesPrice))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.DiscountRate, opt => opt.MapFrom(src => src.DiscountRate))
                .ForMember(dest => dest.InventoryUserId, opt => opt.MapFrom(src => src.InventoryUserId));
           
            CreateMap<ProductPrice, InventoryUserPriceDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ProductPriceId, opt => opt.MapFrom(src => src.Id));

            

            CreateMap<BestSellerProduct, BestSellerProductDto>()
                .ForMember(dest => dest.ProductArabicName, opt => opt.MapFrom(src => src.Product.ArabicName));

            CreateMap<Coupon, CouponDto>().ReverseMap();
            CreateMap<CouponApplicability, CouponApplicabilityDto>().ReverseMap();
            CreateMap<CouponUsage, CouponUsageDto>().ReverseMap();
            CreateMap<BalanceAccount, BalanceAccountDto>().ReverseMap();
            CreateMap<BalanceTransaction, BalanceTransactionDto>().ReverseMap();
            CreateMap<Invoice, InvoiceDto>()
              .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.Id.ToString()))
              .ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => src.InvoiceType.Name))
              .ForMember(dest => dest.CreditUsed, opt => opt.MapFrom(src => src.Order.CreditUsed))
              .ForMember(dest => dest.CashPaid, opt => opt.MapFrom(src => src.Order.CashPaid))
              .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src =>
                  src.Order.CreditUsed > 0 ?
                      (src.Order.CashPaid > 0 ? "Mixed" : "Credit") : "Cash")).ReverseMap();

            CreateMap<CreateCouponDto, Coupon>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid())) // Will be generated by DB
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CouponApplicabilities,
                    opt => opt.MapFrom(src => src.Applicabilities))
                .ForMember(dest => dest.CouponUsages, opt => opt.Ignore()); // Initialize as empty

            // Mapping for CouponApplicability if needed
            CreateMap<CouponApplicabilityDto, CouponApplicability>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CouponId, opt => opt.Ignore());

            // In your mapping profile
            CreateMap<Category, CategoryDto>();
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category))

                .ForMember(dest => dest.Prices,
                    opt => opt.MapFrom(src => src.ProductPrices))

                .ForMember(dest => dest.Inventories,
                    opt => opt.MapFrom(src => src.ProductPrices))

                .ForMember(dest => dest.InventoryUserId,
                    opt => opt.Ignore())

                .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Image) ? null : src.Image));

            

            CreateMap<Invoice, InvoiceReportDto>()
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => src.InvoiceType.Name))
                .ForMember(dest => dest.CreditUsed, opt => opt.MapFrom(src => src.Order.CreditUsed ?? 0))
                .ForMember(dest => dest.CashPaid, opt => opt.MapFrom(src => src.Order.CashPaid ?? 0))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src =>
                        (src.Order.CreditUsed ?? 0) > 0 ?
                            ((src.Order.CashPaid ?? 0) > 0 ? "Mixed" : "Credit") : "Cash"));

            CreateMap<Order, OrderReportDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ((OrderStatus)src.Status).ToString()))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count))
                .ForMember(dest => dest.CreditUsed, opt => opt.MapFrom(src => src.CreditUsed ?? 0))
                .ForMember(dest => dest.CashPaid, opt => opt.MapFrom(src => src.CashPaid ?? 0))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src =>
                    (src.CreditUsed ?? 0) > 0 ?
                        ((src.CashPaid ?? 0) > 0 ? "Mixed" : "Credit") : "Cash"))
                .ForMember(dest => dest.HasInvoice, opt => opt.MapFrom(src => src.Invoices.Any()));

            CreateMap<BalanceAccount, BalanceAccountReportDto>()
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src =>
                    src.AccountType == 0 ? "Cash" : "Credit"))
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.BalanceTransactions.Count))
                .ForMember(dest => dest.TotalDeposits, opt => opt.MapFrom(src =>
                    src.BalanceTransactions.Where(t => t.Amount > 0).Sum(t => t.Amount)))
                .ForMember(dest => dest.TotalWithdrawals, opt => opt.MapFrom(src =>
                    Math.Abs(src.BalanceTransactions.Where(t => t.Amount < 0).Sum(t => t.Amount))));

            CreateMap<BalanceTransaction, BalanceTransactionReportDto>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src =>
                    Enum.GetName(typeof(TransactionType), src.TransactionType)))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src =>
                    src.OrderId.HasValue ? src.OrderId.ToString() : null));

            CreateMap<Product, ProductPrice>()
                   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductPrices.First().Id))
                   .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                   .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                   //.ForMember(dest => dest.MainCategoryId, opt => opt.MapFrom(src => src.MainCategoryId))
                   .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                   .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                   .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                   .ForMember(dest => dest.InventoryUserId, opt => opt.MapFrom(src =>
                        src.ProductPrices != null && src.ProductPrices.Any()
                            ? src.ProductPrices.First().InventoryUserId
                            : Guid.Empty))
                   .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src =>
                        src.ProductPrices != null && src.ProductPrices.Any()
                            ? src.ProductPrices.First().PurchasePrice
                            : 0))
                   .ForMember(dest => dest.SalesPrice, opt => opt.MapFrom(src =>
                        src.ProductPrices != null && src.ProductPrices.Any()
                            ? src.ProductPrices.First().SalesPrice
                            : 0))
                   .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src =>
                        src.ProductPrices != null && src.ProductPrices.Any()
                            ? src.ProductPrices.First().StockQuantity
                            : 0))
                   .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src))
                   .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
           //.ForMember(dest => dest.MainCategory, opt => opt.MapFrom(src => src.MainCategory));

            CreateMap<UpdateProductDto, Product>()
           // Map properties with different names
           //.ForMember(dest => dest.ActiveIngerdientId, opt => opt.MapFrom(src => src.ActiveIngredientId))
           // Ignore properties that shouldn't be mapped
           .ForMember(dest => dest.Image, opt => opt.Ignore())
           .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
           .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
           //.ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
           // Only map if source value is not null
           .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<ProductPrice, ProductInventoryDto>()
                .ForMember(dest => dest.InventoryUserId, opt => opt.MapFrom(src => src.InventoryUserId))
                .ForMember(dest => dest.SalesPrice, opt => opt.MapFrom(src => src.SalesPrice))
                .ForMember(dest => dest.DiscountRate, opt => opt.MapFrom(src => src.DiscountRate))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity));

           
                
                
            CreateMap<ReturnOrder, ReturnOrderDto>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ReturnOrderStatus)src.Statuse))
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ReturnedItems));
			  

            CreateMap<ReturnedItem, ReturnedItemDto>();
            //             .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            //             .ForMember(dest => dest.ReasonName, opt => opt.MapFrom(src => src.Reason.Reason))
            //.ForMember(dest => dest.InventoryName, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));

            // PharmacyDetails mapping
            //CreateMap<PharmacyDetailsDto, PharmacyDetail>()
            //    .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Will be set in service

            //.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }


        //public class RegionResolver : IValueResolver<CreateUserDto, User, Guid?>
        //{
        //    private readonly IRegionService _regionService;

        //    public RegionResolver(IRegionService regionService)
        //    {
        //        _regionService = regionService;
        //    }

        //    public Guid? Resolve(CreateUserDto source, User destination, Guid? destMember, ResolutionContext context)
        //    {
        //        return _regionService.GetRegionIdAsync(
        //            source.RegionName,
        //            source.DesName,
        //            source.GovId,
        //            source.City
        //        ).GetAwaiter().GetResult();
        //    }
        //}
    }
}
