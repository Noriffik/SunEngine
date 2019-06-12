using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SunEngine.Core.Configuration.Options;
using SunEngine.Core.Services.Response.Concrete;

namespace SunEngine.Core.Services
{
    public interface IImagesService
    {
        Task<FileAndDir> SaveImageAsync(IFormFile file, ResizeOptions resizeOptions);
        FileAndDir SaveBitmapImage(Stream stream, ResizeOptions ro, string ext);
    }

    public class ImagesService : IImagesService
    {
        protected static readonly int MaxSvgSizeBytes = 40 * 1024;

        protected static readonly object lockObject = new object();

        protected readonly IImagesNamesService imagesNamesService;
        protected readonly ImagesOptions imagesOptions;
        protected readonly IHostingEnvironment env;

        public ImagesService(IOptions<ImagesOptions> imageOptions, ImagesNamesService imagesNamesService, IHostingEnvironment env)
        {
            this.imagesOptions = imageOptions.Value;
            this.env = env;
            this.imagesNamesService = imagesNamesService;
        }

        private string GetAllowedExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            switch (ext)
            {
                case ".jpeg":
                    return ".jpg";
                case ".jpg":
                case ".png":
                case ".gif":
                    return ext;
            }

            if (imagesOptions.AllowSvgUpload && ext == ".svg")
                return ext;
            
            return null;
        }

        public virtual async Task<FileAndDir> SaveImageAsync(IFormFile file, ResizeOptions resizeOptions)
        {
            var ext = GetAllowedExtension(file.FileName);
            switch (ext)
            {
                case null:
                    throw new ArgumentNullException($"Not allowed extension");
                case ".svg" when file.Length >= MaxSvgSizeBytes:
                    throw new Exception($"Svg max size is {MaxSvgSizeBytes / 1024} kb");
            }

            var fileAndDir = imagesNamesService.GetNewImageNameAndDir(ext);
            var dirFullPath = Path.Combine(env.WebRootPath, imagesOptions.UploadDir, fileAndDir.Dir);
            var fullFileName = Path.Combine(dirFullPath, fileAndDir.File);

            lock (lockObject)
                if (!Directory.Exists(dirFullPath))
                    Directory.CreateDirectory(dirFullPath);

            if (ext == ".svg")
                using (var stream = new FileStream(fullFileName, FileMode.Create))
                    await file.CopyToAsync(stream);
            else
            {
                using (var stream = file.OpenReadStream())
                using (Image<Rgba32> image = Image.Load(stream))
                {
                    image.Mutate(x => x.Resize(resizeOptions));
                    image.Save(fullFileName);
                }
            }

            return fileAndDir;
        }

        public virtual FileAndDir SaveBitmapImage(Stream stream, ResizeOptions ro, string ext)
        {
            using (Image<Rgba32> image = Image.Load(stream))
            {
                var fileAndDir = imagesNamesService.GetNewImageNameAndDir(ext);
                var dirFullPath = Path.Combine(env.WebRootPath, imagesOptions.UploadDir, fileAndDir.Dir);

                lock (lockObject)
                    if (!Directory.Exists(dirFullPath))
                        Directory.CreateDirectory(dirFullPath);

                var fullFileName = Path.Combine(dirFullPath, fileAndDir.File);

                image.Mutate(x => x.Resize(ro));

                image.Save(fullFileName);

                return fileAndDir;
            }
        }
    }
}