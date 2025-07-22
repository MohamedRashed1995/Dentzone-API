using Dragza.Application.Shared;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IProductPriceRepository ProductPriceRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        IReturnedItemRepository ReturnedItemRepository { get; }
        IReturnOrderRepository ReturnOrderRepository { get; }
        IReturnReasonRepository ReturnReasonRepository { get; }
        IRepository<City> CityRepository { get; }
        IRepository<Destrict> DestrictRepository { get; }
        IRegionRepository RegionRepository { get; }
        IPharmacyDetailRepository PharmacyDetailRepository { get; }
        IActiveIngredientRepository ActiveIngredientRepository { get; }
        ICouponRepository CouponRepository { get; }
        ICouponApplicabilityRepository CouponApplicabilityRepository { get; }
        ICouponUsageRepository CouponUsageRepository { get;}
        IBalanceAccountRepository BalanceAccountRepository { get;}
        IBalanceTransactionRepository BalanceTransactionRepository { get;}
        IInvoiceRepository InvoiceRepository { get;}
        IInvoiceTypeRepository InvoiceTypeRepository { get;}
        IMainCategoryRepository MainCategoryRepository { get;}
        IGovernateRepository GovernateRepository { get;}
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();


        // Add other repositories as needed
        Task<int> SaveChangesAsync();
    }
}
