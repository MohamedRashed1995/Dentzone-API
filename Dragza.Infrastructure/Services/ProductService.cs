using AutoMapper;
using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Cryptography;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Dragza.Infrastructure.Services
{

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly DragzaContext _context;
        private readonly string _uploadsFolder;
        private readonly string _baseUrl = "http://dentzone.runasp.net/Uploads/products/";
        private readonly string _baseCategoriesUrl = "http://dentzone.runasp.net/Uploads/categories/";


        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorage,DragzaContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorage = fileStorage;
            _context = context;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "products");
            if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            string? imageName = null;
            if (dto.Photo != null)
            {
                imageName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Photo.FileName)}";
                var filePath = Path.Combine(_uploadsFolder, imageName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Photo.CopyToAsync(stream);
            }
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null) throw new Exception("Category not found");

            
            var product = _mapper.Map<Product>(dto);
            
            
            product.Id = Guid.NewGuid();
            product.ProductCode = GenerateProductCode();
            product.CreatedAt = DateTime.UtcNow;

            product.Image = imageName;
            //product.Description = dto.Description;
            //product.Preef = dto.Preef;
            //product.CategoryId = dto.CategoryId;
            
            
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }


        public async Task<ProductResponseDto> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetProductWithDetailsAsync(id);

            if (product == null)
                throw new Exception($"Product with ID {id} not found");

            // تعديل URLs قبل الـ Mapping لتجنب التكرار
            if (!string.IsNullOrEmpty(product.Image) && !product.Image.StartsWith("http"))
            {
                product.Image = $"{_baseUrl}{product.Image}";
            }

            if (product.Category != null && !string.IsNullOrEmpty(product.Category.ImageName)
                && !product.Category.ImageName.StartsWith("http"))
            {
                product.Category.ImageName = $"{_baseCategoriesUrl}{product.Category.ImageName}";
            }

            // Mapping بعد تعديل الصور
            var productDto = _mapper.Map<ProductResponseDto>(product);

            return productDto;
        }

        //public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(bool includeDeleted , string search, int page = 1, int size = 10)
        //{
        //    var products = await _unitOfWork.ProductRepository.GetAllProductsWithDetailsAsync(includeDeleted ,search,page,size);
        //    return _mapper.Map<List<ProductResponseDto>>(products);
        //}

        //public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(bool includeDeleted, string search, int page = 1, int size = 10)
        //{
        //    var products = await _unitOfWork.ProductRepository.GetAllProductsWithDetailsAsync(includeDeleted, search, page, size);

        //    // AutoMapper هيملى Inventories صح
        //    var productDtos = _mapper.Map<List<ProductResponseDto>>(products);

        //    return productDtos;
        //}


        //public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(bool includeDeleted, string search, int page = 1, int size = 10)
        //{
        //    var products = await _unitOfWork.ProductRepository
        //        .GetAllProductsWithDetailsAsync(includeDeleted, search, page, size);

        //    var productDtos = _mapper.Map<List<ProductResponseDto>>(products);

        //    return productDtos;
        //}

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(
                bool includeDeleted, string search, int page = 1, int size = 10)
        {
            // جلب المنتجات من الـ Repository
            var products = await _unitOfWork.ProductRepository
                .GetAllProductsWithDetailsAsync(includeDeleted, search, page, size);

            // تعديل URLs قبل الـ Mapping لتجنب التكرار
            products.ForEach(p =>
            {
                // Product Image
                if (!string.IsNullOrEmpty(p.Image) && !p.Image.StartsWith("http"))
                {
                    p.Image = $"{_baseUrl}{p.Image}";
                }

                // Category Image
                if (p.Category != null && !string.IsNullOrEmpty(p.Category.ImageName)
                    && !p.Category.ImageName.StartsWith("http"))
                {
                    p.Category.ImageName = $"{_baseCategoriesUrl}{p.Category.ImageName}";
                }
            });

            // Mapping بعد تعديل الصور
            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);

            return productDtos;
        }

        public async Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
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

            //if (dto.ActiveIngredientId.HasValue)
            //{
            //    var ingredient = await _unitOfWork.ActiveIngredientRepository.GetByIdAsync(dto.ActiveIngredientId.Value);
            //    if (ingredient == null) throw new Exception("Active ingredient not found");
            //    product.ActiveIngerdientId = dto.ActiveIngredientId.Value;
            //}

            //if (dto.Photo != null)
            //{
            //    product.Image = await _fileStorage.SaveFileAsync(
            //           dto.Photo);
            //}
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return await GetProductByIdAsync(id);
        }

        public async Task SoftDeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
                throw new Exception($"Product with ID {id} not found");

            //product.IsDeleted = true;
            //product.DeletedDate = DateTime.UtcNow;
            
            //update => Delete
            _unitOfWork.ProductRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreProductAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
                throw new Exception($"Product with ID {id} not found");

            

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }
        //public async Task<IEnumerable<ProductResponseDto>> GetProductsByActiveIngredientAsync(Guid activeIngredientId)
        //{
        //    var ingredient = await _unitOfWork.ActiveIngredientRepository.GetByIdAsync(activeIngredientId);
        //    if (ingredient == null)
        //    {
        //        throw new Exception($"Active ingredient with ID {activeIngredientId} not found");
        //    }

        //    var products = await _unitOfWork.ProductRepository.GetByActiveIngredientAsync(activeIngredientId);
        //    return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        //}

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

        public async Task<IEnumerable<ProductBestPriceDto>> GetProductsByCategoryAsync(Guid categoryId)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsByCategoryIdAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductBestPriceDto>>(products);
        }

        public async Task<List<ProductPrice>> GetPricesWithAllProductByInventoryId(Guid inventoryId)
        {
            var products =  await _unitOfWork.ProductRepository.GetPricesWithAllProductByInventoryId(inventoryId);
            var productPrice = _mapper.Map<List<ProductPrice>>(products); // This line now works correctly

            return productPrice;
        }

        public async Task<Product> GetproductbyName(string name)
        {
            return await _unitOfWork.ProductRepository.GetProductByName(name);
            
        }

        public async Task<IEnumerable<ProductsDto>> GetProductsAsync(string name)
        {
            var products = await _unitOfWork.ProductRepository.GetProductsAsync(name);
            var productdto = _mapper.Map<List<ProductsDto>>(products); // This line now works correctly

            return productdto;
        }

        public async Task<Product> AddProduct(ProductAddDto productDto)
        {
            var product1 = _mapper.Map<Product>(productDto);


          

          
            product1.ProductCode = GenerateProductCode();

            var product =await _unitOfWork.ProductRepository.AddProduct(product1);

            return product;
           
        }


        public static int GenerateProductCode()
        {
            return RandomNumberGenerator
                .GetInt32(10000000, 99999999);
               
        }

        public async Task<Product> GetproductbyCode(int productCode)
        {
            return await _unitOfWork.ProductRepository.GetProductByCode(productCode);
        }
    }
}
