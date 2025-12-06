using Microsoft.AspNetCore.Mvc;
using ProductService.Dtos;
using ProductService.Services;

namespace ProductService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO product)
        {
            await _productService.AddProductAsync(product);
            return Ok();
        }

        [HttpPatch("{id}")] 
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {
            var updatedProduct = await _productService.UpdateProductAsync(productDto,id);
            if (updatedProduct == false)
            {
                return NotFound();
            }
            return Ok(updatedProduct);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await Task.Run(() => _productService.DeleteProductAsync(id));
            return NoContent();
        }

    }
}
