using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {         
        private readonly ProductDBContext _context;

        public ProductRepository(ProductDBContext context)
        {
            _context = context;
        }


        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task  DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);// in ef core call dbcontext.save changes.
                await _context.SaveChangesAsync();

            }
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            // Simulate async operation for demonstration purposes
            //return await Task.FromResult(_products);
            return await _context.Products.ToListAsync();
        }
       
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);

        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var existingProduct = await GetProductByIdAsync(product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
