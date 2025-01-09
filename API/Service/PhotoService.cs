using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IO;
using System;
using System.Threading.Tasks;

namespace API.Service
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IWebHostEnvironment _environment;

        public PhotoService(IOptions<CloudinarySettings> config,
            IWebHostEnvironment environment)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret                
                );
            _cloudinary = new Cloudinary(acc);
            _environment = environment;
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
             var uploadResult= new ImageUploadResult();

            if (file.Length>0)
            {
                using var strem = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName,strem),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "Event"
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeleteAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file."); 
          
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

           
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

           
            var fileUrl = $"/uploads/{fileName}"; // Relative URL
            return fileUrl;
        }
    }
}
