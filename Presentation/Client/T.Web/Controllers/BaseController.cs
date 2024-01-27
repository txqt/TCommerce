using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace T.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        protected void SetStatusMessage(string message)
        {
            TempData["StatusMessage"] = message;
        }
        protected void AddErrorsFromModel(ModelStateDictionary.ValueEnumerable values)
        {
            foreach (ModelStateEntry modelState in values)
                foreach (ModelError error in modelState.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage.ToString());

                }
        }
        public async Task<string> RenderViewComponentAsync(Type viewComponent, object args = null)
        {

            var sp = HttpContext.RequestServices;

            var helper = new DefaultViewComponentHelper(
                sp.GetRequiredService<IViewComponentDescriptorCollectionProvider>(),
                HtmlEncoder.Default,
                sp.GetRequiredService<IViewComponentSelector>(),
                sp.GetRequiredService<IViewComponentInvokerFactory>(),
                sp.GetRequiredService<IViewBufferScope>());

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, NullView.Instance, ViewData, TempData, writer, new HtmlHelperOptions());
                helper.Contextualize(context);
                var result = await helper.InvokeAsync(viewComponent, args);
                result.WriteTo(writer, HtmlEncoder.Default);
                await writer.FlushAsync();
                return writer.ToString();
            }
        }
        public static async Task<string> RenderViewAsync(string viewName, ControllerContext controllerContext, object model, bool isPartial = false)
        {
            var actionContext = controllerContext as ActionContext;

            var serviceProvider = controllerContext.HttpContext.RequestServices;
            var razorViewEngine = serviceProvider.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            var tempDataProvider = serviceProvider.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            using (var sw = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(actionContext, viewName, !isPartial);

                if (viewResult?.View == null)
                    throw new ArgumentException($"{viewName} does not match any available view");

                var viewDictionary =
                    new ViewDataDictionary(new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    { Model = model };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
        public partial class NullView : IView
        {
            public static readonly NullView Instance = new();

            public string Path => string.Empty;

            public Task RenderAsync(ViewContext context)
            {
                ArgumentNullException.ThrowIfNull(context);

                return Task.CompletedTask;
            }
        }
        public static int GetNumberFromPrefix(string input, string prefix)
        {
            if (input.StartsWith(prefix))
            {
                string numberString = input.Substring(prefix.Length);

                if (int.TryParse(numberString, out int number))
                {
                    return number;
                }
            }
            return -1; // Trả về -1 nếu không thể chuyển đổi thành số
        }
    }
}
