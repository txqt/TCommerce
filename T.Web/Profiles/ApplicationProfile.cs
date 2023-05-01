using AutoMapper;
using T.Library.Model;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;

namespace T.Web.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Product, ProductUpdateViewModel>().ReverseMap();
            CreateMap<ProductAttributeMapping, ProductAttributeMappingModel>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductAttributeId, opt => opt.MapFrom(src => src.ProductAttributeId))
                .ForMember(dest => dest.TextPrompt, opt => opt.MapFrom(src => src.TextPrompt))
                .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => src.IsRequired))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.ValidationMinLength, opt => opt.MapFrom(src => src.ValidationMinLength))
                .ForMember(dest => dest.ValidationMaxLength, opt => opt.MapFrom(src => src.ValidationMaxLength))
                .ForMember(dest => dest.ValidationFileAllowedExtensions, opt => opt.MapFrom(src => src.ValidationFileAllowedExtensions))
                .ForMember(dest => dest.ValidationFileMaximumSize, opt => opt.MapFrom(src => src.ValidationFileMaximumSize))
                .ForMember(dest => dest.DefaultValue, opt => opt.MapFrom(src => src.DefaultValue))
                //.ForMember(dest => dest.ValidationRulesString, opt => opt.MapFrom(src => src.ValidationRulesString))
                .ForMember(dest => dest.ProductAttributeName, opt => opt.Ignore())
                .ForSourceMember(dest => dest.ProductAttribute, opt => opt.DoNotValidate())
                .ReverseMap()
                ;
        }
    }
}
