namespace T.Web.Areas.Admin.Models
{
    public class DataTableResponse
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<object> Data { get; set; }
        public string Error { get; set; }
    }
}
