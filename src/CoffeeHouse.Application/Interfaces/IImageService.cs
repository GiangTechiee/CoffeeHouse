using Microsoft.AspNetCore.Http;

namespace CoffeeHouse.Application.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Uploads an image to ImageKit and returns the URL or unique file name
    /// </summary>
    /// <param name="file">The image file to upload</param>
    /// <param name="folder">Optional folder name in ImageKit</param>
    /// <returns>The URL of the uploaded image</returns>
    Task<string> UploadImageAsync(IFormFile file, string folder = "products");

    /// <summary>
    /// Deletes an image from ImageKit
    /// </summary>
    /// <param name="fileId">The unique FileId from ImageKit</param>
    Task DeleteImageAsync(string fileId);
}
