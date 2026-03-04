using Dragza.Application.Data;
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
        DragzaContext DbContext { get; }
        IUserRepository UserRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IAddressRepository AddressRepository { get; }
        IProductPriceRepository ProductPriceRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        IReturnedItemRepository ReturnedItemRepository { get; }
        IReturnOrderRepository ReturnOrderRepository { get; }
        IReturnReasonRepository ReturnReasonRepository { get; }
        
        ICouponRepository CouponRepository { get; }
        ICouponApplicabilityRepository CouponApplicabilityRepository { get; }
        ICouponUsageRepository CouponUsageRepository { get;}
        IBalanceAccountRepository BalanceAccountRepository { get;}
        IBalanceTransactionRepository BalanceTransactionRepository { get;}
        IInvoiceRepository InvoiceRepository { get;}
        IInvoiceTypeRepository InvoiceTypeRepository { get;}
        //IMainCategoryRepository MainCategoryRepository { get;}
       
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();


        // Add other repositories as needed
        Task<int> SaveChangesAsync();
    }
}
