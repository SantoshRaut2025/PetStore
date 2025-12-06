using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using ProductService.Dtos;
using ProductService.Models;
using ProductService.Repositories;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        

        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
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
            try
            {
                var products = await _productRepository.GetAllProductsAsync();
                return products.Select(p => ConvertToDTO(p)).ToList();
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
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product != null)
            {
                return ConvertToDTO(product);
            }
            return null;
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
