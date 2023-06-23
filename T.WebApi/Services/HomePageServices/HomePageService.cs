namespace T.WebApi.Services.HomePageServices
{
    public interface IHomePageService
    {
        Task<List<string>> GetAllCategoryShowOnHomePage();
    }
    public class HomePageService : IHomePageService
    {
    }
}
