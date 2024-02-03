public class ProductParameters : QueryStringParameters
{
    public List<int> ids { get; set; } = null;
    public int CategoryId { get; set; }
    public int ManufacturerId { get; set; }
}