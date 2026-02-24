using CoffeeHouse.Application.Interfaces;
using Imagekit.Sdk;
using Imagekit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoffeeHouse.Infrastructure.Services;

public class ImageKitService : IImageService
{
    private readonly ImagekitClient _imageKitClient;
    private readonly ILogger<ImageKitService> _logger;
    private readonly string _urlEndpoint;

    public ImageKitService(IConfiguration configuration, ILogger<ImageKitService> logger)
    {
        _logger = logger;
        var publicKey = configuration["ImageKitSettings:PublicKey"];
        var privateKey = configuration["ImageKitSettings:PrivateKey"];
        _urlEndpoint = configuration["ImageKitSettings:UrlEndpoint"];

        if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(_urlEndpoint))
        {
            _logger.LogWarning("ImageKit settings are not fully configured in appsettings.json");
        }

        // Namespace: Imagekit.Sdk, Class: ImagekitClient
        _imageKitClient = new ImagekitClient(publicKey, privateKey, _urlEndpoint);
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder = "products")
    {
        return await Task.Run(async () => {
            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";

                var uploadRequest = new FileCreateRequest
                {
                    file = fileBytes,
                    fileName = fileName,
                    folder = folder,
                    useUniqueFileName = true
                };

                // Sync method in SDK 4.0.1
                var response = _imageKitClient.Upload(uploadRequest);

                if (response != null && !string.IsNullOrEmpty(response.url))
                {
                    _logger.LogInformation("Successfully uploaded image to ImageKit: {FileId}", response.fileId);
                    return response.url;
                }

                _logger.LogError("Failed to upload image to ImageKit. Response was null or missing URL.");
                throw new Exception("Image upload failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to ImageKit");
                throw;
            }
        });
    }

    public async Task DeleteImageAsync(string fileId)
    {
        if (string.IsNullOrEmpty(fileId)) return;

        await Task.Run(() => {
            try
            {
                // Sync method in SDK 4.0.1
                _imageKitClient.DeleteFile(fileId);
                _logger.LogInformation("Delete request sent to ImageKit for FileId: {FileId}", fileId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image from ImageKit: {FileId}", fileId);
            }
        });
    }
}
