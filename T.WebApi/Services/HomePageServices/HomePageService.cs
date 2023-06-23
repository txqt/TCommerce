namespace T.WebApi.Services.HomePageServices
{
    public interface IHomePageService
    {
        Task<List<string>> GetAllCategoryShowOnHomePage();
    }
    public class HomePageService : IHomePageService
    {
        public Task<List<string>> GetAllCategoryShowOnHomePage()
        {
            throw new NotImplementedException();
        }
    }
}
