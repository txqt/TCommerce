using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace T.Web.Helpers.TagHelpers
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

        // Thay đổi CustomStyle thành asp-style trực tiếp
        [HtmlAttributeName("asp-style")]
        public string Style { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Sử dụng Guid.NewGuid().ToString("N") để tạo chuỗi GUID không có dấu gạch ngang
            Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToString("N") : Id;

            output.TagName = "div";
            output.Attributes.Add("class", "accordion");

            // Kiểm tra và sử dụng giá trị mặc định nếu CustomStyle là null
            output.Attributes.Add("style", Style ?? "margin-top: 36px");

            var accordionItemDiv = new TagBuilder("div");
            accordionItemDiv.Attributes.Add("class", "accordion-item");

            var cardDiv = new TagBuilder("div");
            cardDiv.Attributes.Add("class", "card");

            var accordionHeader = new TagBuilder("h2");
            accordionHeader.Attributes.Add("class", "accordion-header");

            if (!string.IsNullOrEmpty(Title))
            {
                var button = new TagBuilder("button");
                button.Attributes.Add("class", "accordion-button");
                button.Attributes.Add("type", "button");
                button.Attributes.Add("data-bs-toggle", "collapse");
                button.Attributes.Add("data-bs-target", $"#{Id}");
                button.InnerHtml.Append(Title);
                button.Attributes.Add("aria-expanded", (!Collapsed).ToString().ToLower());

                accordionHeader.InnerHtml.AppendHtml(button);
            }

            var cardBodyDiv = new TagBuilder("div");
            cardBodyDiv.Attributes.Add("class", "card-body");

            var accordionCollapse = new TagBuilder("div");
            accordionCollapse.Attributes.Add("id", Id);
            accordionCollapse.Attributes.Add("class", Collapsed ? "accordion-collapse collapse" : "accordion-collapse collapse show");

            // Lấy nội dung bên trong thẻ <t-accordion>
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
