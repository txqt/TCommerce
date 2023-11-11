using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.Library.Model.Response;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.PictureServices
{
    public interface IPictureService
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
        Task<ServiceResponse<Picture>> GetPictureByIdAsync(int pictureId);
        Task<ServiceResponse<bool>> DeletePictureByIdAsync(int pictureId);
    }

    public class PictureService : IPictureService
    {
        private readonly IHostEnvironment _environment;
        private readonly IRepository<Picture> _pictureRepository;

        public PictureService(IHostEnvironment environment, IRepository<Picture> pictureRepository)
        {
            _environment = environment;
            _pictureRepository = pictureRepository;
        }

        public async Task<ServiceResponse<int>> SavePictureWithEncryptFileName(IFormFile file)
        {
            try
            {
                var imageFile = file;
                if (imageFile.Length > 0)
                {
                    var uniqueFileName = Path.GetRandomFileName();
                    var fileExtension = Path.GetExtension(imageFile.FileName);
                    var newFileName = uniqueFileName + fileExtension;

                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/", newFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var picture = new Picture
                    {
                        UrlPath = "/uploads/" + newFileName
                    };
                    await _pictureRepository.CreateAsync(picture);

                    var pictureId = (await _pictureRepository.Table.FirstOrDefaultAsync(x=>x.UrlPath.Contains(newFileName))).Id;

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

        public async Task<ServiceResponse<int>> SavePictureWithoutEncryptFileName(IFormFile file)
        {
            try
            {
                var imageFile = file;
                if (imageFile.Length > 0)
                {
                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/", imageFile.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var picture = new Picture
                    {
                        UrlPath = "/uploads/" + imageFile.FileName
                    };
                    await _pictureRepository.CreateAsync(picture);

                    await _pictureRepository.CreateAsync(picture);

                    var pictureId = (await _pictureRepository.Table.FirstOrDefaultAsync(x => x.UrlPath.Contains(imageFile.FileName))).Id;

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

        public async Task<ServiceResponse<Picture>> GetPictureByIdAsync(int pictureId)
        {
            return new ServiceSuccessResponse<Picture>() { Data = await _pictureRepository.GetByIdAsync(pictureId) };
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
