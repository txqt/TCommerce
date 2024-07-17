using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Web.Models;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Component
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

            var productBoxListModel = new List<ProductBoxModel>();

            foreach(var item in productsHomePage)
            {
                productBoxListModel.Add(await _productModelService.PrepareProductBoxModel(item, null));
            }

            if (productsHomePage is not null)
            {
                homepagelist = new List<HomePageModel>()
                {
                    new HomePageModel()
                    {
                        Title = "Featured",
                        ProductList = productBoxListModel
                    }
                };
            }

            return View(homepagelist);
        }
    }
}
