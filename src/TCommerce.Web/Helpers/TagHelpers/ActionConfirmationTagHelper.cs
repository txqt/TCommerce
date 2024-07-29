using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TCommerce.Web.Helpers.TagHelpers
{
    [HtmlTargetElement("t-action-confirmation")]
    public class ActionConfirmationTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-button-id")]
        public string ButtonId { get; set; }

        [HtmlAttributeName("asp-messsage")]
        public string Message { get; set; } = "Are you sure you want to proceed?";

        [HtmlAttributeName("asp-confirm-button-text")]
        public string ConfirmButtonText { get; set; } = "Yes";

        [HtmlAttributeName("asp-cancel-button-text")]
        public string CancelButtonText { get; set; } = "No";
        public string FormId { get; set; } // Add this property

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "modal fade");
            output.Attributes.SetAttribute("id", $"{ButtonId}-action-confirmation");
            output.Attributes.SetAttribute("tabindex", "-1");
            output.Attributes.SetAttribute("role", "dialog");
            output.Attributes.SetAttribute("aria-labelledby", $"{ButtonId}ModalLabel");
            output.Attributes.SetAttribute("aria-hidden", "true");

            var modalHtml = $@"
            <div class='modal-dialog' role='document'>
                <div class='modal-content'>
                    <div class='modal-header'>
                        <h5 class='modal-title' id='{ButtonId}ModalLabel'>Confirmation</h5>
                        <button type='button' class='close' data-dismiss='modal' aria-label='Close'>
                            <span aria-hidden='true'>&times;</span>
                        </button>
                    </div>
                    <div class='modal-body'>
                        {Message}
                    </div>
                    <div class='modal-footer'>
                        <button type='button' class='btn btn-secondary' data-dismiss='modal'>{CancelButtonText}</button>
                        <button type='submit' class='btn btn-primary' id='{ButtonId}-action-confirmation-submit-button'>{ConfirmButtonText}</button>
                    </div >
                </div >
            </div > ";

            var scriptHtml = $@"
<script>
    $(function() {{
        $('#{ButtonId}').attr('data-toggle', 'modal').attr('data-target', '#{ButtonId}-action-confirmation');
        $('#{ButtonId}-action-confirmation-submit-button').attr('name', $('#{ButtonId}').attr('name'));
        $('#{ButtonId}').attr('name', '');
        if ($('#{ButtonId}').attr('type') == 'submit')
            $('#{ButtonId}').attr('type', 'button');
    }});
</script>";

            output.Content.SetHtmlContent(modalHtml + scriptHtml);
        }
    }
}
