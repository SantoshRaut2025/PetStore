using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ProductService.Dtos;
using ProductService.Models;
using ProductService.Repositories;
using System.Text.Json;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IMapper mapper, IProductRepository productRepository, ICacheService cacheService, ILogger<ProductService> logger)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public Product ConvertToProduct(ProductDTO dto)
        {
            return _mapper.Map<Product>(dto);
        }

        public ProductDTO ConvertToDTO(Product product)
        {
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<List<ProductDTO>> GetAllProductsAsync()
        {
            string cacheKey = "all_products";
            try
            {
                _logger.LogInformation("Trying to get from Redis cache");
                var products = await _cacheService.GetAsync<List<ProductDTO>>(cacheKey);
                if(products == null)
                {
                     _logger.LogInformation("Cache MISS - Fetching from database");
                    var productEntities = await _productRepository.GetAllProductsAsync();
                    products = [.. productEntities.Select(p => ConvertToDTO(p))];
                    await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromMinutes(10));
                }
                else
                {
                    _logger.LogInformation("Cache HIT - Returning from Redis");
                }
                return products;
            }   
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting from Redis cache for key: {Key}", cacheKey);
                return [];
            }
        }

        public async Task AddProductAsync(ProductDTO dto)
        {
            var product = ConvertToProduct(dto);
            await _productRepository.AddProduct(product); 
             // Invalidate cache
            await _cacheService.RemoveAsync("all_products");           
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            string cacheKey = $"Product_{id}";
            var productDto = await _cacheService.GetAsync<ProductDTO>(cacheKey);
            if (productDto == null)
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                    return null;
                productDto = ConvertToDTO(product);
                await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(10));
            }
            
            return productDto;
        }

        public async Task<bool> UpdateProductAsync(ProductDTO dto, int id)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct != null)
            {
                existingProduct.Name = dto.Name;
                existingProduct.Price = dto.Price;
                existingProduct.Description = dto.Description;
                // Invalidate specific and list caches
                await _cacheService.RemoveAsync($"Product_{id}");
                await _cacheService.RemoveAsync("all_products");
                return await _productRepository.UpdateProductAsync(existingProduct);
            }
            return false;
        }

        public async Task DeleteProductAsync(int id)
        {
            // Invalidate specific and list caches
            await _cacheService.RemoveAsync($"Product_{id}");
            await _cacheService.RemoveAsync("all_products");
            await _productRepository.DeleteProduct(id);// Simulate async operation
        }
    }
}
