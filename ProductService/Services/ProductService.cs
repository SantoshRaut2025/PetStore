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
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _distCache;

        public ProductService(IMapper mapper, IProductRepository productRepository, IMemoryCache cache, IDistributedCache distCache)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _cache = cache;
            _distCache = distCache;
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
                var cachedData = await _distCache.GetStringAsync(cacheKey);
                var cachedProducts = _cache.Get<List<ProductDTO>>(cacheKey);

                if (cachedData !=null || cachedProducts !=null)

                {
                    return JsonSerializer.Deserialize<List<ProductDTO>>(cachedData);
                }
                else
                {
                    var products = await _productRepository.GetAllProductsAsync();
                    var productDTOs = products.Select(p => ConvertToDTO(p)).ToList();
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    { 
                        AbsoluteExpirationRelativeToNow = (TimeSpan.FromMinutes(10)),
                        SlidingExpiration = (TimeSpan.FromMinutes(5))
                    };
                    _cache.Set(cacheKey, productDTOs, cacheEntryOptions);
                    _distCache.SetString(cacheKey, JsonSerializer.Serialize(productDTOs), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    });
                    return productDTOs;
                }
            }   
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw;
            }
        }

        public async Task AddProductAsync(ProductDTO dto)
        {
            
            var product = ConvertToProduct(dto);
            await _productRepository.AddProduct(product);            
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            string cacheKey = $"Product_{id}";

            if (!_cache.TryGetValue(cacheKey, out ProductDTO? productDto))
            {
               var  product = await _productRepository.GetProductByIdAsync(id);
                if(product == null)
                    return null;
                productDto = ConvertToDTO(product);
                _cache.Set(cacheKey, productDto, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });               
              
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
                return await _productRepository.UpdateProductAsync(existingProduct);
            }
            return false;
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteProduct(id);// Simulate async operation
        }
    }
}
