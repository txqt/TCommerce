using Microsoft.AspNetCore.Mvc;
using T.Web.Areas.Admin.Services.PrepareAdminModel;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.ProductService;

namespace T.Web.Component
{
    public class RelatedProductsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IProductModelService _productModelFactory;

        public RelatedProductsViewComponent(IProductService productService, IProductModelService productModelFactory)
        {
            _productService = productService;
            _productModelFactory = productModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            //load and cache report
            var productIds = (await _productService.GetRelatedProductsByProductId1Async(productId)).Select(x => x.ProductId2).ToList();

            //load products
            var products = (await _productService.GetProductsByIdsAsync(productIds))?
                //availability dates
                .Where(p => _productService.ProductIsAvailable(p)).ToList();

            if (!products.Any())
                return Content(string.Empty);

            var model = (await Task.WhenAll(products.Select(async product => await _productModelFactory.PrepareProductBoxModel(product, null)))).ToList();

            return View(model);
        }
    }
}
