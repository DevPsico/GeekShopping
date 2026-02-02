using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;

namespace GeekShopping.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public const string BasePath = "/api/product";

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Cria um novo produto enviando os dados via POST
        public async Task<ProductModel> CreateProduct(ProductModel product)
        {
            var response = await _httpClient.PostAsJsonAsync(BasePath, product);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentSafeAsync<ProductModel>();
            else
                throw new Exception("Erro ao criar produto.");
        }

        // Deleta um produto pelo ID via DELETE
        public async Task<bool> DeleteProduct(long id)
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentSafeAsync<bool>();
            else 
                throw new Exception("Erro ao deletar produto.");


            return response.IsSuccessStatusCode;
        }

        // Busca todos os produtos via GET
        public async Task<IEnumerable<ProductModel>> FindAllProducts()
        {
            var response = await _httpClient.GetAsync(BasePath);
            return await response.ReadContentSafeAsync<List<ProductModel>>();
        }

        // Busca um produto específico pelo ID via GET
        public async Task<ProductModel> FindByIdProduct(long id)
        {
            var response = await _httpClient.GetAsync($"{BasePath}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentSafeAsync<ProductModel>();
            else
                throw new Exception("Produto não encontrado.");
        }

        // Atualiza um produto via PUT
        public async Task<ProductModel> UpdateProduct(ProductModel product)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/{product.Id}", product);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentSafeAsync<ProductModel>();
            else
                throw new Exception("Erro ao atualizar produto.");
        }
    }
}