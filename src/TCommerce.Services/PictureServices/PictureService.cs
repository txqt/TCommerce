using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Response;
using TCommerce.Services.IRepositoryServices;

namespace TCommerce.Services.PictureServices
{
    public class PictureService : IPictureService
    {
        private readonly IHostEnvironment _environment;
        private readonly IRepository<Picture> _pictureRepository;
        private string ClientUrl;
        private readonly IConfiguration _configuration;

        public PictureService(IHostEnvironment environment, IRepository<Picture> pictureRepository, IConfiguration configuration)
        {
            _environment = environment;
            _pictureRepository = pictureRepository;
            _configuration = configuration;
            ClientUrl = _configuration.GetSection("Url:ClientUrl").Value!;
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

                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/uploads/", newFileName);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var picture = new Picture
                    {
                        UrlPath = "/images/uploads/" + newFileName
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
                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/uploads/", fileName);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var picture = new Picture
                    {
                        UrlPath = "/images/uploads/" + fileName
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
            if (picture is not null && picture.UrlPath is not null && !picture.UrlPath.Contains(ClientUrl))
            {
                picture.UrlPath = ClientUrl + picture.UrlPath;
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
