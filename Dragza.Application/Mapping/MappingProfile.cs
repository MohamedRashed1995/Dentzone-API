using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.DTO.Order;
using Dragza.Domain.DTO.ReturnOrder;
using Dragza.Domain.Enum;
using Dragza.Domain.Models;

namespace Dragza.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.RegionId, opt => opt.MapFrom<RegionResolver>())
                .ForMember(dest => dest.NomalizedUserName,
                    opt => opt.MapFrom(src => src.UserName.ToUpper()))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(_ => true));
            CreateMap<PharmacyDetailsDto, PharmacyDetail>();

            CreateMap<User, UserResponseDto>();
            CreateMap<PharmacyDetail, PharmacyDetailsDto>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<CreateProductDto, Product>();

            // ProductPrice mappings
            CreateMap<ProductPrice, ProductPriceResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.InventoryUserName, opt => opt.MapFrom(src => src.InventoryUser.UserName));

            CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (OrderStatus)src.Status))
            .ForMember(dest => dest.InventoryName, opt => opt.MapFrom(src => src.InventoryUser.BussinesName))
            .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.PharmacyUser.BussinesName))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.ProductPrice.PurchasePrice));
            CreateMap<OrderItemDto, OrderItem>();

            CreateMap<ReturnOrder, ReturnOrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (ReturnOrderStatus)src.Statuse))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ReturnedItems));

            CreateMap<ReturnedItem, ReturnedItemDto>()
                .ForMember(dest => dest.ReasonId, opt => opt.MapFrom(src => src.Reason.Id));

            CreateMap<ReturnReason, ReturnReasonDto>();

            CreateMap<User, UserDto>()
            .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.BussinesName))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Region.RegionName));
            CreateMap<Product, ProductResponseDto>()
           .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.ProductPrices));

            CreateMap<ProductPrice, ProductPriceResponseDto>();
            CreateMap<ActiveIngredient, ActiveIngredientDto>();

            CreateMap<ProductPrice, ProductBestPriceDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.BestSalesPrice, opt => opt.MapFrom(src => src.SalesPrice))
            .ForMember(dest => dest.PriceId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PriceDate, opt => opt.MapFrom(src => src.CreationDate));

            CreateMap<CreateProductPriceDto, ProductPrice>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryUserId, opt => opt.Ignore());

            CreateMap<User, UserWithPharmacyDto>()
           .ForMember(dest => dest.PharmacyDetails, opt => opt.MapFrom(src =>
            src.IsPharmacy == true ? src.PharmacyDetailUsers.FirstOrDefault() : null));


            CreateMap<User, UserWithPharmacyDto>()
           .ForMember(dest => dest.PharmacyDetails, opt => opt.MapFrom(src =>
               src.IsPharmacy == true
               ? src.PharmacyDetailUsers.FirstOrDefault()
               : null));

            // PharmacyDetail to PharmacyDetailsDto mapping
            CreateMap<PharmacyDetail, PharmacyDetailsResponseDto>()
                .ForMember(dest => dest.CommercialRegisteryAttachPath,
                    opt => opt.MapFrom(src => src.CommercialRegisteryAttach))
                .ForMember(dest => dest.NationalIdAttachPath,
                    opt => opt.MapFrom(src => src.NationalIdAttach))
                .ForMember(dest => dest.PharmacyLicenseAttachPath,
                    opt => opt.MapFrom(src => src.PharmacyLicenseAttach))
                .ForMember(dest => dest.OwnersgipAttachPath,
                    opt => opt.MapFrom(src => src.OwnersgipAttach))
                .ForMember(dest => dest.TaxationCardAttachPath,
                    opt => opt.MapFrom(src => src.TaxationCardAttach));

            CreateMap<ProductPrice, ProductPriceDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.InventoryUser, opt => opt.MapFrom(src => src.InventoryUser))
                .ForMember(dest => dest.ProductPriceId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ProductPrice, InventoryUserPriceDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ProductPriceId, opt => opt.MapFrom(src => src.Id));

            CreateMap<BestSellerProduct, BestSellerProductDto>();

            CreateMap<Coupon, CouponDto>().ReverseMap();
            CreateMap<CouponApplicability, CouponApplicabilityDto>().ReverseMap();
            CreateMap<CouponUsage, CouponUsageDto>().ReverseMap();
            CreateMap<BalanceAccount, BalanceAccountDto>().ReverseMap();
            CreateMap<BalanceTransaction, BalanceTransactionDto>().ReverseMap();
            CreateMap<Invoice, InvoiceDto>()
              .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.Id.ToString()))
              .ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => src.InvoiceType.Name))
              .ForMember(dest => dest.PharmacyName, opt => opt.MapFrom(src => src.Order.PharmacyUser.BussinesName))
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
            CreateMap<MainCategory, MainCategoryDto>();
            CreateMap<MainCategory, MainCategoryWithRelationsDto>();
            CreateMap<CreateMainCategoryDto, MainCategory>();
            CreateMap<UpdateMainCategoryDto, MainCategory>();


            CreateMap<Region, RegionDto>();
            CreateMap<Region, RegionWithUsersDto>();
            CreateMap<CreateRegionDto, Region>();
            CreateMap<UpdateRegionDto, Region>();
        }


        public class RegionResolver : IValueResolver<CreateUserDto, User, Guid?>
        {
            private readonly IRegionService _regionService;

            public RegionResolver(IRegionService regionService)
            {
                _regionService = regionService;
            }

            public Guid? Resolve(CreateUserDto source, User destination, Guid? destMember, ResolutionContext context)
            {
                return _regionService.GetRegionIdAsync(
                    source.RegionName,
                    source.DesName,
                    source.GovId,
                    source.City
                ).GetAwaiter().GetResult();
            }
        }
    }
}
