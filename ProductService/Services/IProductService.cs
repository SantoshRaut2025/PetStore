using ProductService.Dtos;

namespace ProductService.Services
{
    public interface IProductService
    {
        
        Task<List<ProductDTO>> GetAllProductsAsync();
        Task AddProductAsync(ProductDTO dto);

        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<bool> UpdateProductAsync(ProductDTO dto, int id);
        Task DeleteProductAsync(int id);
    }
}
