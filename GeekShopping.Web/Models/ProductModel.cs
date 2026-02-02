using System.ComponentModel.DataAnnotations;

namespace GeekShopping.Web.Models
{
    public enum CategoriaProduto
    {
        Blusa,
        Short,
        Calcado,
        Eletronico,
        Livros,
        Calca
    }

    public class ProductModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public CategoriaProduto Category { get; set; }

        [Required(ErrorMessage = "A URL da imagem é obrigatória.")]
        [Url(ErrorMessage = "Informe uma URL válida.")]
        public string ImageUrl { get; set; }
    }
}