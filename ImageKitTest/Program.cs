using System;
using System.IO;
using Imagekit.Sdk;
using Imagekit.Models;
using Microsoft.Extensions.Configuration;

public class Program {
    public static async System.Threading.Tasks.Task Main() {
        try {
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "src", "CoffeeHouse.Web", "appsettings.json");
            var configuration = new ConfigurationBuilder().AddJsonFile(configPath).Build();
            ImagekitClient imagekit = new ImagekitClient(configuration["ImageKitSettings:PublicKey"], configuration["ImageKitSettings:PrivateKey"], configuration["ImageKitSettings:UrlEndpoint"]);

            string localImagePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "src", "CoffeeHouse.Web", "wwwroot", "img", "products", "latte-nong.jpeg");
            byte[] fileBytes = File.ReadAllBytes(localImagePath);
            
            FileCreateRequest request = new FileCreateRequest {
                file = fileBytes,
                fileName = $"coffee_test.jpeg",
                folder = "test_upload",
                useUniqueFileName = true
            };

            var response = imagekit.Upload(request);
            if (response != null) {
                // Write ONLY the URL to a clean file
                File.WriteAllText("url.txt", response.url);
                Console.WriteLine("DONE_URL");
            }
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
