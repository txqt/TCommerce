using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Web.Models;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Component
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

            var model = new List<ProductBoxModel>();

            foreach (var product in products)
            {
                model.Add(await _productModelFactory.PrepareProductBoxModel(product, null));
            }

            return View(model);
        }
    }
}
