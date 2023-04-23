using AutoMapper;
using T.Library.Model;
using T.Library.Model.ViewsModel;

namespace T.Web.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Product, ProductUpdateViewModel>();
        }
    }
}
