using System.Drawing.Printing;

namespace T.Web.Areas.Admin.Models
{
    public class DataTableViewModel
    {
        public DataTableViewModel()
        {
            TableId = Guid.NewGuid().ToString();
        }

        public string TableId { get; set; }
        public string TableTitle { get; set; }
        public string CreateUrl { get; set; }
        public string CreateButtonName { get; set; } = null;
        public List<string> Headers { get; set; }
        public List<ColumnDefinition> Columns { get; set; }
        public string GetDataUrl { get; set; }
        public bool PopupWindow { get; set; } = false;
        public List<int> LengthMenu { get; set; } = new List<int>() { 10, 25, 50, 100, 200 };
        public int PageLength { get; set; } = 10;
        public bool ServerSideProcessing { get; set; } = false;
    }
    public class ColumnDefinition
    {
        public string Data { get; set; }
        public string EditUrl { get; set; }
        public string DeleteUrl { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
        public bool IsCheckBox { get; set; }
        public bool IsPicture { get; set; }
    }
}
