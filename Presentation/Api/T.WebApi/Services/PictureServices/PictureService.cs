using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.WebApi.Helpers;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.PictureServices
{
    public interface IPictureService : IPictureServiceCommon
    {
        /// <summary>
        /// Saves an image from the IFormFile object with an encrypted file name.
        /// </summary>
        /// <param name="file">IFormFile object containing the image to be saved.</param>
        /// <returns>ServiceResponse<bool> indicating the result of the storage operation.</returns>
        Task<ServiceResponse<int>> SavePictureWithEncryptFileName(IFormFile file);

        /// <summary>
        /// Saves an image from the IFormFile object without encrypting the file name.
        /// </summary>
        /// <param name="file">IFormFile object containing the image to be saved.</param>
        /// <returns>ServiceResponse<bool> indicating the result of the storage operation.</returns>
        Task<ServiceResponse<int>> SavePictureWithoutEncryptFileName(IFormFile file);

        Task<ServiceResponse<bool>> DeletePictureByIdAsync(int pictureId);
    }

    public class PictureService : IPictureService
    {
        private readonly IHostEnvironment _environment;
        private readonly IRepository<Picture> _pictureRepository;
        private string APIUrl;
        private readonly IConfiguration _configuration;

        public PictureService(IHostEnvironment environment, IRepository<Picture> pictureRepository, IConfiguration configuration)
        {
            _environment = environment;
            _pictureRepository = pictureRepository;
            _configuration = configuration;
            APIUrl = _configuration.GetSection("Url:APIUrl").Value!;
        }

        public async Task<ServiceResponse<int>> SavePictureWithEncryptFileName(IFormFile file)
        {
            try
            {
                var imageFile = file;
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = Path.GetRandomFileName();
                    var fileExtension = Path.GetExtension(imageFile.FileName);
                    var newFileName = uniqueFileName + fileExtension;

                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/", newFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var picture = new Picture
                    {
                        UrlPath = "/images/" + newFileName
                    };
                    await _pictureRepository.CreateAsync(picture);

                    var savedPicture = await _pictureRepository.Table.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.UrlPath) && x.UrlPath.Contains(newFileName));
                    if (savedPicture != null)
                    {
                        var pictureId = savedPicture.Id;
                        return new ServiceSuccessResponse<int>() { Data = pictureId };
                    }
                }

                return new ServiceErrorResponse<int>();

            }
            catch
            {
                return new ServiceErrorResponse<int>();
            }
        }

        public async Task<ServiceResponse<int>> SavePictureWithoutEncryptFileName(IFormFile imageFile)
        {
            try
            {
                var pictureId = 0; // Giả sử một giá trị mặc định
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = imageFile.FileName;
                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var picture = new Picture
                    {
                        UrlPath = "/images/" + fileName
                    };
                    await _pictureRepository.CreateAsync(picture);

                    var savedPicture = await _pictureRepository.Table.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.UrlPath) && x.UrlPath.Contains(fileName));
                    if (savedPicture != null)
                    {
                        pictureId = savedPicture.Id;
                    }
                    return new ServiceSuccessResponse<int>() { Data = pictureId };
                }
                else
                {
                    return new ServiceErrorResponse<int>();
                }
            }
            catch
            {
                return new ServiceErrorResponse<int>();
            }
        }

        public async Task<Picture?> GetPictureByIdAsync(int pictureId)
        {
            var picture = await _pictureRepository.GetByIdAsync(pictureId);
            if (picture is not null && picture.UrlPath is not null && !picture.UrlPath.Contains(APIUrl))
            {
                picture.UrlPath = APIUrl + picture.UrlPath;
            }
            return picture;
        }

        public async Task<ServiceResponse<bool>> DeletePictureByIdAsync(int pictureId)
        {
            try
            {
                await _pictureRepository.DeleteAsync(pictureId);
                return new ServiceSuccessResponse<bool>();
            }
            catch
            {
                return new ServiceErrorResponse<bool>();
            }
        }
    }
}
