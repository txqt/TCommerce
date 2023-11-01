using AutoMapper;
using T.Library.Model;
using T.Library.Model.Common;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;

namespace T.Web.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Product, ProductModel>()
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                //.ForMember(dest => dest.FullDescription, opt => opt.MapFrom(src => src.FullDescription))
                //.ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src => src.ShortDescription))
                //.ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                //.ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                //.ForMember(dest => dest.OldPrice, opt => opt.MapFrom(src => src.OldPrice))
                //.ForMember(dest => dest.MarkAsNew, opt => opt.MapFrom(src => src.MarkAsNew))
                //.ForMember(dest => dest.MarkAsNewStartDateTimeUtc, opt => opt.MapFrom(src => src.MarkAsNewStartDateTimeUtc))
                //.ForMember(dest => dest.MarkAsNewEndDateTimeUtc, opt => opt.MapFrom(src => src.MarkAsNewEndDateTimeUtc))
                //.ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
                //.ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
                //.ForMember(dest => dest.VisibleIndividually, opt => opt.MapFrom(src => src.VisibleIndividually))
                .ReverseMap();
            CreateMap<ProductAttributeMappingModel, ProductAttributeMapping>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
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
                .ForPath(dest => dest.ProductAttribute.Name, opt => opt.MapFrom(src => src.ProductAttributeName))
                .AfterMap((src, dest) => dest.ProductAttribute = null)
                .ReverseMap();

            CreateMap<ProductAttributeValue, ProductAttributeValueModel>()
                .ForMember(dest => dest.ProductAttributeMappingId, opt => opt.MapFrom(src => src.ProductAttributeMappingId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ColorSquaresRgb, opt => opt.MapFrom(src => src.ColorSquaresRgb))
                .ForMember(dest => dest.PriceAdjustment, opt => opt.MapFrom(src => src.PriceAdjustment))
                .ForMember(dest => dest.PriceAdjustmentUsePercentage, opt => opt.MapFrom(src => src.PriceAdjustmentUsePercentage))
                .ForMember(dest => dest.WeightAdjustment, opt => opt.MapFrom(src => src.WeightAdjustment))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
                .ForMember(dest => dest.CustomerEntersQty, opt => opt.MapFrom(src => src.CustomerEntersQty))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.IsPreSelected, opt => opt.MapFrom(src => src.IsPreSelected))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
                .ReverseMap();
            CreateMap<Category, CategoryModel>()
                .ReverseMap();
            CreateMap<UserModel, UserViewModel>()
                .ReverseMap();
            CreateMap<ProductCategoryModel, ProductCategory>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.IsFeaturedProduct, opt => opt.MapFrom(src => src.IsFeaturedProduct))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
