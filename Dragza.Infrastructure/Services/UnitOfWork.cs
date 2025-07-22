using Dragaza.Infrastructure.Repositories;
using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DragzaContext _context;
        private IUserRepository _userRepository;
        private IDbContextTransaction _transaction;


        public UnitOfWork(DragzaContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public IRepository<Role> RoleRepository => new Repository<Role>(_context);
        public IProductPriceRepository ProductPriceRepository => new ProductPriceRepository(_context);
        public IProductRepository ProductRepository => new ProductRepository(_context);
        public ICategoryRepository CategoryRepository => new CategoryRepository(_context);
        public IOrderRepository OrderRepository => new OrderRepository(_context);
        public IOrderItemRepository OrderItemRepository => new OrderItemRepository(_context);
        public IReturnReasonRepository ReturnReasonRepository => new ReturnReasonRepository(_context);
        public IReturnedItemRepository ReturnedItemRepository => new ReturnedItemRepository(_context);
        public IReturnOrderRepository ReturnOrderRepository => new ReturnOrderRepository(_context);
        public IRepository<City> CityRepository => new Repository<City>(_context);
        public IRepository<Destrict> DestrictRepository => new Repository<Destrict>(_context);
        public IRegionRepository RegionRepository => new RegionRepository(_context);

        public IPharmacyDetailRepository PharmacyDetailRepository => new PharmacyDetailRepository(_context);
        public IActiveIngredientRepository ActiveIngredientRepository => new ActiveIngredientRepository(_context);
        public ICouponRepository CouponRepository => new CouponRepository(_context);
        public ICouponApplicabilityRepository CouponApplicabilityRepository => new CouponApplicabilityRepository(_context);
        public ICouponUsageRepository CouponUsageRepository => new CouponUsageRepository(_context);
        public IBalanceAccountRepository BalanceAccountRepository => new BalanceAccountRepository(_context);
        public IBalanceTransactionRepository BalanceTransactionRepository => new BalanceTransactionRepository(_context);
        public IInvoiceRepository InvoiceRepository => new InvoiceRepository(_context);
        public IInvoiceTypeRepository InvoiceTypeRepository => new InvoiceTypeRepository(_context);
        public IMainCategoryRepository MainCategoryRepository => new MainCategoryRepository(_context);
        public IGovernateRepository GovernateRepository => new GovernateRepository(_context);

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitAsync()
        {
        
                await _context.SaveChangesAsync();
             
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
