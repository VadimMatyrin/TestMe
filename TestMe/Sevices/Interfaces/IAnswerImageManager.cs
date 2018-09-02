using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Sevices.Interfaces
{
    public interface IAnswerImageManager
    {
        Task<bool> SaveAnswerImageAsync(IFormFile file, string imageName);
        bool DeleteAnswerImage(string imageName);

    }
}
