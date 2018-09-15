using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestMe.Config;

namespace TestMe.Data.Extentions
{
    public static class HttpPostedFileBaseExtensions
    { 
        public static bool IsImage(this IFormFile postedFile, IOptions<PhotoConfig> photoConfig)
        {
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }
                if (postedFile.Length < photoConfig.Value.MinSize || postedFile.Length > photoConfig.Value.MaxSize)
                {
                    return false;
                }

                var buffer = new byte[photoConfig.Value.MinSize];
                postedFile.OpenReadStream().Read(buffer, 0, photoConfig.Value.MinSize);
                var content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            postedFile.OpenReadStream().Position = 0;

            return true;
        }
    }
}
