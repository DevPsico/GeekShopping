using GeekShopping.ProductAPI.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.ProductAPI.Data.Dto
{
    public class ProductDto
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public CategoriaProduto Category { get; set; }
        public string ImageUrl { get; set; }
    }
}
