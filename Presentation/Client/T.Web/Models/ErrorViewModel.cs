namespace T.Web.Models
{
    public class ErrorViewModel
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}