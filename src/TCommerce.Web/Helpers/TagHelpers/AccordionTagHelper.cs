using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TCommerce.Web.Helpers.TagHelpers
{
    [HtmlTargetElement("t-accordion")]
    public class AccordionTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-id")]
        public string Id { get; set; }

        [HtmlAttributeName("asp-title")]
        public string Title { get; set; }

        [HtmlAttributeName("asp-collapsed")]
        public bool Collapsed { get; set; } = false;

        [HtmlAttributeName("asp-style")]
        public string Style { get; set; }

        [HtmlAttributeName("asp-data-attributes")]
        public Dictionary<string, string> DataAttributes { get; set; } = new Dictionary<string, string>();

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToString("N") : Id;

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "accordion");
            output.Attributes.SetAttribute("style", Style ?? "margin-top: 36px");
            foreach (var dataAttribute in DataAttributes)
            {
                output.Attributes.Add($"data-{dataAttribute.Key}", dataAttribute.Value);
            }

            var accordionItemDiv = new TagBuilder("div");
            accordionItemDiv.AddCssClass("accordion-item");

            var cardDiv = new TagBuilder("div");
            cardDiv.AddCssClass("card");

            var accordionHeader = new TagBuilder("h2");
            accordionHeader.AddCssClass("accordion-header");

            if (!string.IsNullOrEmpty(Title))
            {
                var button = new TagBuilder("button");
                button.AddCssClass("accordion-button");
                button.Attributes.Add("type", "button");
                button.Attributes.Add("data-bs-toggle", "collapse");
                button.Attributes.Add("data-bs-target", $"#{Id}");
                button.Attributes.Add("aria-expanded", (!Collapsed).ToString().ToLower());
                button.InnerHtml.Append(Title);

                accordionHeader.InnerHtml.AppendHtml(button);
            }

            var cardBodyDiv = new TagBuilder("div");
            cardBodyDiv.AddCssClass("card-body");

            var accordionCollapse = new TagBuilder("div");
            accordionCollapse.Attributes.Add("id", Id);
            accordionCollapse.AddCssClass(Collapsed ? "accordion-collapse collapse" : "accordion-collapse collapse show");

            var content = await output.GetChildContentAsync();
            cardBodyDiv.InnerHtml.AppendHtml(content);
            accordionCollapse.InnerHtml.AppendHtml(cardBodyDiv);

            cardDiv.InnerHtml.AppendHtml(accordionHeader);
            cardDiv.InnerHtml.AppendHtml(accordionCollapse);

            accordionItemDiv.InnerHtml.AppendHtml(cardDiv);
            output.Content.AppendHtml(accordionItemDiv);
        }
    }
}
