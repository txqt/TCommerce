namespace T.Web.Areas.Admin.Models
{
    public class DataTableViewModel
    {
        public string TableId { get; set; }
        public string TableTitle { get; set; }
        public string CreateUrl { get; set; }
        public List<string> Headers { get; set; }
        public List<ColumnDefinition> Columns { get; set; }
        public string GetDataUrl { get; set; }
    }
    public class ColumnDefinition
    {
        public string Data { get; set; }
        public string EditUrl { get; set; }
        public string DeleteUrl { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
    }
}
