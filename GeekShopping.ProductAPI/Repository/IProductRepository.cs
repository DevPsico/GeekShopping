using GeekShopping.ProductAPI.Data.Dto;

namespace GeekShopping.ProductAPI.Repository
{
    public interface IProductRepository
    {

        Task<IEnumerable<ProductDto>> FindAll();
        Task<ProductDto> FindById(long id);
        Task<ProductDto> Create(ProductDto product);
        Task<ProductDto> Update(ProductDto product);
        Task<bool> Delete(long id);



    }
}
