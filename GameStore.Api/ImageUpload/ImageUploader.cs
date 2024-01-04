using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace GameStore.Api.ImageUpload;

public class ImageUploader : IImageUploader
{
    private readonly BlobContainerClient _containerClient;

    public ImageUploader(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = _containerClient.GetBlobClient(file.FileName);

        await blobClient.DeleteIfExistsAsync();

        using var fileStream = file.OpenReadStream();

        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });

        return blobClient.Uri.ToString();
    }
}
