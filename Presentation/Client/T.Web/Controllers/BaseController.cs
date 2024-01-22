using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace T.Web.Controllers
{
    public class BaseController : Controller
    {
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
        public async Task<string> RenderViewComponent(Type viewComponent, object args = null)
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
    }
}
