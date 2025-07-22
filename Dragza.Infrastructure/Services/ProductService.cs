using AutoMapper;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Dragza.Infrastructure.Services
{

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;


        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null) throw new Exception("Category not found");

            if (dto.ActiveIngredientId.HasValue)
            {
                var ingredient = await _unitOfWork.ActiveIngredientRepository.GetByIdAsync(dto.ActiveIngredientId.Value);
                if (ingredient == null) throw new Exception("Active ingredient not found");
            }

            var product = _mapper.Map<Product>(dto);
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;
            product.ActiveIngerdientId = dto.ActiveIngredientId;
            if (dto.Photo != null)
            {
                product.Image = await _fileStorage.SaveFileAsync(
                       dto.Photo);
            }
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductWithDetailsAsync(id);
            if (product == null || product.IsDeleted == true)
                throw new Exception($"Product with ID {id} not found");

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(bool includeDeleted , string search)
        {
            var products = await _unitOfWork.ProductRepository.GetAllProductsWithDetailsAsync(includeDeleted ,search);
            return _mapper.Map<List<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null || product.IsDeleted == true)
                throw new Exception($"Product with ID {id} not found");

            _mapper.Map(dto, product);
            product.UpdatedAt = DateTime.UtcNow;
            product.Id = id; // Ensure the ID remains the same
            if (dto.Photo != null)
            {
                var filePath = await _fileStorage.SaveFileAsync(
                       dto.Photo);
                product.Image = filePath;
            }
            if (dto.CategoryId.HasValue)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(dto.CategoryId.Value);
                if (category == null) throw new Exception("Category not found");
                product.CategoryId = dto.CategoryId.Value;
            }

            if (dto.ActiveIngredientId.HasValue)
            {
                var ingredient = await _unitOfWork.ActiveIngredientRepository.GetByIdAsync(dto.ActiveIngredientId.Value);
                if (ingredient == null) throw new Exception("Active ingredient not found");
                product.ActiveIngerdientId = dto.ActiveIngredientId.Value;
            }

            if (dto.Photo != null)
            {
                product.Image = await _fileStorage.SaveFileAsync(
                       dto.Photo);
            }
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return await GetProductByIdAsync(id);
        }

        public async Task SoftDeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null || product.IsDeleted == true)
                throw new Exception($"Product with ID {id} not found");

            product.IsDeleted = true;
            product.DeletedDate = DateTime.UtcNow;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreProductAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null || !product.IsDeleted == true)
                throw new Exception($"Product with ID {id} not found");

            product.IsDeleted = false;
            product.DeletedDate = null;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<ProductResponseDto>> GetProductsByActiveIngredientAsync(Guid activeIngredientId)
        {
            var ingredient = await _unitOfWork.ActiveIngredientRepository.GetByIdAsync(activeIngredientId);
            if (ingredient == null)
            {
                throw new Exception($"Active ingredient with ID {activeIngredientId} not found");
            }

            var products = await _unitOfWork.ProductRepository.GetByActiveIngredientAsync(activeIngredientId);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<IEnumerable<BestSellerProductDto>> GetBestSellingProductsAsync(int topN = 10)
        {
            var bestSellers = await _unitOfWork.OrderItemRepository.GetBestSellingProductsAsync(topN);
            return _mapper.Map<IEnumerable<BestSellerProductDto>>(bestSellers);
            //// Get completed orders (assuming status 5 is "Completed")
            //var completedOrderIds = await _unitOfWork.OrderRepository
            //    .GetCompletedOrderIdsAsync();

            //var bestSellers = await _unitOfWork.OrderItemRepository
            //    .GetBestSellingProductsQuery(completedOrderIds)
            //    .Take(topN)
            //    .ToListAsync();

            //return _mapper.Map<IEnumerable<BestSellerProductDto>>(bestSellers);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(Guid categoryId)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsByCategoryIdAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<List<ProductPrice>> GetPricesWithAllProductByInventoryId(Guid inventoryId)
        {
            var products =  await _unitOfWork.ProductRepository.GetPricesWithAllProductByInventoryId(inventoryId);
            var productPrice = _mapper.Map<List<ProductPrice>>(products); // This line now works correctly

            return productPrice;
        }
    }
}
