public class ProductParameters : QueryStringParameters
{
    public string OrderBy { get; set; } = "name";
    public string searchText { get; set; } = null;
}