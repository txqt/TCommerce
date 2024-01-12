using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Web.Helpers;

namespace T.Web.Services.PictureServices
{
    public interface IPictureService : IPictureServiceCommon
    {

    }
    public class PictureService : HttpClientHelper, IPictureService
    {
        public PictureService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Picture> GetPictureByIdAsync(int pictureId)
        {
            return await GetAsync<Picture>($"api/pictures/{pictureId}");
        }
    }
}
