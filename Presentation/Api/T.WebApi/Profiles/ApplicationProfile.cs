using AutoMapper;
using T.Library.Model;
using T.Library.Model.Banners;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;

namespace T.WebApi.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src => src.ShortDescription))
                .ForMember(dest => dest.FullDescription, opt => opt.MapFrom(src => src.FullDescription))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.OldPrice, opt => opt.MapFrom(src => src.OldPrice))
                .ForMember(dest => dest.MarkAsNew, opt => opt.MapFrom(src => src.MarkAsNew))
                .ForMember(dest => dest.MarkAsNewEndDateTimeUtc, opt => opt.MapFrom(src => src.MarkAsNewEndDateTimeUtc))
                .ForMember(dest => dest.MarkAsNewStartDateTimeUtc, opt => opt.MapFrom(src => src.MarkAsNewStartDateTimeUtc))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
                .ForMember(dest => dest.VisibleIndividually, opt => opt.MapFrom(src => src.VisibleIndividually))
                .ForMember(dest => dest.AdminComment, opt => opt.MapFrom(src => src.AdminComment))
                .ForMember(dest => dest.ShowOnHomepage, opt => opt.MapFrom(src => src.ShowOnHomepage))
                .ForMember(dest => dest.MetaKeywords, opt => opt.MapFrom(src => src.MetaKeywords))
                .ForMember(dest => dest.MetaDescription, opt => opt.MapFrom(src => src.MetaDescription))
                .ForMember(dest => dest.MetaTitle, opt => opt.MapFrom(src => src.MetaTitle))
                .ForMember(dest => dest.AllowUserReviews, opt => opt.MapFrom(src => src.AllowUserReviews))
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku))
                .ForMember(dest => dest.IsShipEnabled, opt => opt.MapFrom(src => src.IsShipEnabled))
                .ForMember(dest => dest.IsFreeShipping, opt => opt.MapFrom(src => src.IsFreeShipping))
                .ForMember(dest => dest.OrderMinimumQuantity, opt => opt.MapFrom(src => src.OrderMinimumQuantity))
                .ForMember(dest => dest.OrderMaximumQuantity, opt => opt.MapFrom(src => src.OrderMaximumQuantity))
                .ForMember(dest => dest.NotReturnable, opt => opt.MapFrom(src => src.NotReturnable))
                .ForMember(dest => dest.DisableBuyButton, opt => opt.MapFrom(src => src.DisableBuyButton))
                .ForMember(dest => dest.AvailableForPreOrder, opt => opt.MapFrom(src => src.AvailableForPreOrder))
                .ForMember(dest => dest.PreOrderAvailabilityStartDateTimeUtc, opt => opt.MapFrom(src => src.PreOrderAvailabilityStartDateTimeUtc))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => src.Length))
                .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.Width))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.AvailableStartDateTimeUtc, opt => opt.MapFrom(src => src.AvailableStartDateTimeUtc))
                .ForMember(dest => dest.AvailableEndDateTimeUtc, opt => opt.MapFrom(src => src.AvailableEndDateTimeUtc))
                .ReverseMap();

            CreateMap<User, UserModel>()
                .ReverseMap();

            CreateMap<BannerViewModel, Banner>()
                .ReverseMap();
        }
    }
}
