using GeekShopping.ProductAPI.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.ProductAPI.Model
{
    public enum CategoriaProduto
    {
        Blusa,
        Short,
        Calcado,
        Eletronico,
        Livros,
        Calca,
    }


    [Table("product")]
    public class Product : BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("category")]
        public CategoriaProduto Category { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }
    }
}
