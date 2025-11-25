using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    // ProductPriceService.cs
    public class ProductPriceService : IProductPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductPriceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductPriceResponseDto> CreateProductPriceAsync(CreateProductPriceDto dto, Guid userId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(dto.CategoryId);
            var productPrice = await _unitOfWork.ProductPriceRepository.GetPricesByInventoryAndProduct(userId,dto.ProductId);


            if (product == null || category == null)
                throw new KeyNotFoundException("Product or Category not found");

            if (productPrice != null)
            {
                productPrice.PurchasePrice = dto.PurchasePrice;
                productPrice.SalesPrice = dto.SalesPrice;
                productPrice.StockQuantity = dto.StockQuantity;

                _unitOfWork.ProductPriceRepository.Update(productPrice);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<ProductPriceResponseDto>(productPrice);
            }

            var price = _mapper.Map<ProductPrice>(dto);
            price.Id = Guid.NewGuid();
            price.CreationDate = DateTime.UtcNow;
            price.InventoryUserId = userId;

            await _unitOfWork.ProductPriceRepository.AddAsync(price);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductPriceResponseDto>(price);
        }

        public async Task<IEnumerable<ProductPriceResponseDto>> GetPricesByUserAsync(Guid userId)
        {
            var prices = await _unitOfWork.ProductPriceRepository.GetPricesByInventoryUserAsync(userId);
            return _mapper.Map<IEnumerable<ProductPriceResponseDto>>(prices);
        }

        public async Task<IEnumerable<ProductBestPriceDto>> GetProductsBestPricesAsync(int page = 1, int size = 10)
        {
            var bestPrices = await _unitOfWork.ProductPriceRepository.GetBestPricesAsync(page,size);
            return _mapper.Map<IEnumerable<ProductBestPriceDto>>(bestPrices);
        }

        public async Task<IEnumerable<ProductBestPriceDto>> GetProductsBestPricesSortingAsync(int sort, int page = 1, int size = 10)
        {
            var bestPrices = await _unitOfWork.ProductPriceRepository.GetBestPricesSortingAsync(sort, page, size );
            return _mapper.Map<IEnumerable<ProductBestPriceDto>>(bestPrices);
        }

        public async Task<IEnumerable<ProductPriceDetailsDto>> GetPricesByCategoryAsync(Guid categoryId)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                throw new Exception($"Category with ID {categoryId} not found");

            var prices = await _unitOfWork.ProductPriceRepository.GetPricesWithDetailsByCategory(categoryId);
            return _mapper.Map<IEnumerable<ProductPriceDetailsDto>>(prices);
        }

        public async Task<IEnumerable<ProductPriceDetailsDto>> GetPricesByProductAsync(Guid productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null || product.IsDeleted == true)
                throw new Exception($"Product with ID {productId} not found");

            var prices = await _unitOfWork.ProductPriceRepository.GetPricesByProduct(productId);
            return _mapper.Map<IEnumerable<ProductPriceDetailsDto>>(prices);
        }

        public async Task<IEnumerable<InventoryUserPriceDetailsDto>> GetPricesByInventoryUserAsync(Guid userId)
        {
            // Verify user exists and has inventory role
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("Inventory user not found");

            var prices = await _unitOfWork.ProductPriceRepository.GetPricesByInventoryUser(userId);
            return _mapper.Map<IEnumerable<InventoryUserPriceDetailsDto>>(prices);
        }

        public async Task UpdateProductPriceAndQuantityAsync(Guid productId, Guid productPriceId, decimal salesPrice, decimal purchasePrice, int quantity ,int maxQuantity,Guid userId)
            {
            if (productPriceId != Guid.Empty && productPriceId != null)
            {
                var productPrice = await _unitOfWork.ProductPriceRepository.GetByIdAsync(productPriceId);

                if (productPrice != null)
                {
                    // Update existing
                    productPrice.SalesPrice = salesPrice;
                    productPrice.PurchasePrice = purchasePrice;
                    productPrice.StockQuantity = quantity;
                    productPrice.MaxQuantity = maxQuantity;
                    productPrice.UpdatedDate = DateTime.UtcNow;

                }
            }
            else
            {

                if (productId == Guid.Empty)
                    throw new ArgumentException("Product ID cannot be empty", nameof(productId));
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product != null)
                {
                    // Create new
                    
                    var productPrice = new ProductPrice
                    {
                        Id = productPriceId,
                        ProductId = productId,
                        SalesPrice = salesPrice,
                        PurchasePrice = purchasePrice,
                        StockQuantity = quantity,
                        CreationDate = DateTime.UtcNow,
                        IsDeleted = false,
                        InventoryUserId = userId,
                       
                        
                    };
                    await _unitOfWork.ProductPriceRepository.AddAsync(productPrice);
                }
            }

            await _unitOfWork.CommitAsync();
        }

		public async Task<IEnumerable<ProductPriceResponseDto>> GetAllProductPricesAsync(bool includeDeleted, string search, int page = 1, int size = 10)
		{
			var productPrices = await _unitOfWork.ProductPriceRepository.GetAllProductPricesWithDetailsAsync(includeDeleted, search,page,size);
			return _mapper.Map<List<ProductPriceResponseDto>>(productPrices);
		}

        public async Task<IEnumerable<ProductBestPriceDto>> GetProductByCategoryAsync(Guid categoryId)
        {
            var productPrices = await _unitOfWork.ProductPriceRepository.GetProductByCategory(categoryId);
            return _mapper.Map<List<ProductBestPriceDto>>(productPrices);
        }

        public async Task<IEnumerable<ProductPriceResponseDto>> GetAllProductAsync(Guid productId)
        {
            var productPrices = await _unitOfWork.ProductPriceRepository.GetAllProductAsync(productId);
            return _mapper.Map<List<ProductPriceResponseDto>>(productPrices);
        }
    }
}
