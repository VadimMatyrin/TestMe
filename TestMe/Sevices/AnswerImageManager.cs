using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestMe.Config;
using TestMe.Data.Extentions;
using TestMe.Sevices.Interfaces;

namespace TestMe.Sevices
{
    public class AnswerImageManager : IAnswerImageManager
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IOptions<PhotoConfig> _photoConfig; 
        public AnswerImageManager(IHostingEnvironment appEnvironment, IOptions<PhotoConfig> photoConfig)
        {
            _appEnvironment = appEnvironment;
            _photoConfig = photoConfig;
        }
        public bool DeleteAnswerImage(string imageName)
        {
            var filePath = Path.Combine(_appEnvironment.WebRootPath, $"{_photoConfig.Value.PhotoPath}\\{imageName}");
            var file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
                return true;
            }
            return false;
        }

        public async Task<bool> SaveAnswerImageAsync(IFormFile image, string imageName)
        {
            if (image is null || imageName is null)
                throw new ArgumentNullException();

            if (image.IsImage(_photoConfig))
            {
                var uploads = Path.Combine(_appEnvironment.WebRootPath, _photoConfig.Value.PhotoPath);
                using (var fileStream = new FileStream(Path.Combine(uploads, imageName), FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                    return true;
                }
            }
            return false;
                
        }
    }
}
