using AutoMapper;
using ProductService.Models;
using System.ComponentModel.DataAnnotations;


namespace ProductService.Dtos
{
    public class ProductDTO
    {       
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
    }   

    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Map ProductDTO → Product
            CreateMap<ProductDTO, Product>().ReverseMap();
           

            // Map Product → ProductDTO (optional, if you need reverse mapping)
            //CreateMap<Product, ProductDTO>();
        }
    }

}
