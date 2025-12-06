using ProductService.Models;

namespace ProductService.Repositories
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        public Task<List<Product>> GetAllProductsAsync();
        public Task<Product?> GetProductByIdAsync(int id);
        Task<bool> UpdateProductAsync(Product product);
        Task DeleteProduct(int id);

    }
}
