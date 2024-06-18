using AutoMapper;
using apifinal.BaseDados.Models;
using apifinal.Services.DTOs;
using System;

namespace apifinal.Services.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductDTO, TbProduct>();
            CreateMap<ProductUpdateDTO, TbProduct>();
            CreateMap<TbProduct, ProductBaseDTO>();
            CreateMap<ProductStockUpdateDTO, TbProduct>();
            CreateMap<PromotionDTO, TbPromotion>()
            .ForMember(dest => dest.Startdate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Startdate, DateTimeKind.Unspecified)))
            .ForMember(dest => dest.Enddate, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Enddate, DateTimeKind.Unspecified)));
            CreateMap<SalesDTO, TbSale>()
            .ForMember(dest => dest.Price, opt => opt.Ignore())
            .ForMember(dest => dest.Discount, opt => opt.Ignore())
            .ForMember(dest => dest.Code, opt => opt.Ignore())
            .ForMember(dest => dest.Createat, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.CreateAt, DateTimeKind.Unspecified)));


        }
    }
}
