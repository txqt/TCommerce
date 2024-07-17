using Microsoft.AspNetCore.Http;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Response;

namespace TCommerce.Core.Interface
{
    public interface IPictureService
    {
        Task<Picture?> GetPictureByIdAsync(int pictureId);
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
}
