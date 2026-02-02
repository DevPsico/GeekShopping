using AutoMapper;

namespace GeekShopping.ProductAPI.Config
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Model.Product, Data.Dto.ProductDto>().ReverseMap();
        }
    }
}