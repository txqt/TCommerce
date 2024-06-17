using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.Web.Models;
using T.Web.Services.PictureServices;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.ProductService;
using T.Web.Services.UrlRecordService;
using static T.Web.Models.HomePageModel;

namespace T.Web.Component
{
    public class HomePageProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IProductModelService _productModelService;
        public HomePageProductsViewComponent(IProductService productService, IProductModelService productModelService)
        {
            _productService = productService;
            _productModelService = productModelService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var productsHomePage = await _productService.GetAllProductsDisplayedOnHomepageAsync();

            var homepagelist = new List<HomePageModel>();

            if (productsHomePage is not null)
            {
                homepagelist = new List<HomePageModel>()
                {
                    new HomePageModel()
                    {
                        Title = "Featured",
                        ProductList = (await Task.WhenAll(productsHomePage.Select(async product => await _productModelService.PrepareProductBoxModel(product, null)))).ToList()
                    }
                };
            }

            return View(homepagelist);
        }
    }
}
