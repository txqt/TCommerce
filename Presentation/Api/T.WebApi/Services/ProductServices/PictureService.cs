using T.Library.Model.Response;
using T.Library.Model;
using T.WebApi.Database.ConfigurationDatabase;
using Microsoft.EntityFrameworkCore;

namespace T.WebApi.Services.ProductService
{
    public interface IPictureService
    {
        Task<ServiceResponse<bool>> UploadFiles(List<IFormFile> formFiles);
    }
    public class PictureService : IPictureService
    {
        private readonly DatabaseContext _context;
        private readonly IHostEnvironment _environment;

        public PictureService(DatabaseContext context, IHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<ServiceResponse<bool>> UploadFiles(List<IFormFile> formFiles)
        {
            string path = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                if (formFiles != null)
                {
                    foreach (var FileUpload in formFiles)
                    {
                        var file = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads", FileUpload.FileName);
                        using (var fileStream = new FileStream(file, FileMode.Create))
                        {
                            await FileUpload.CopyToAsync(fileStream);
                        }
                    }
                }
                return new ServiceResponse<bool>() { Message = "File upload successfully", Success = true };
            }
            catch(Exception ex)
            {
                return new ServiceResponse<bool>() { Message = ex.Message, Success = false };
            }
        }
    }
}
