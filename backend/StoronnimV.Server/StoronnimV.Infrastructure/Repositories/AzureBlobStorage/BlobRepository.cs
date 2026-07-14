using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using StoronnimV.Domain.Contracts.AzureBlobStorage;

namespace StoronnimV.Infrastructure.Repositories.AzureBlobStorage;

public class BlobRepository : IBlobRepository
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobRepository()
    {
        string? connectionString = Environment.GetEnvironmentVariable("BLOB_STORAGE");
        
        _blobServiceClient = new BlobServiceClient(connectionString);
    }
    
    public async Task<string> AddFileAndGetUrlAsync(
        string containerName,
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken ct)
    {
        BlobContainerClient? container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(cancellationToken: ct);
        
        BlobClient? blobClient = container.GetBlobClient(fileName);
        
        await blobClient.UploadAsync(fileStream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        }, ct);
        
        return blobClient.Uri.ToString();
    }

    public string GetFileUrl(string containerName, string fileName, CancellationToken ct)
    {
        BlobContainerClient? blobClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient? blob = blobClient.GetBlobClient(fileName);
        
        return blob.Uri.ToString();
    }

    public async Task DeleteFileAsync(string containerName, string fileName, CancellationToken ct)
    {
        BlobContainerClient? container = _blobServiceClient.GetBlobContainerClient(containerName);
        
        BlobClient? blobClient = container.GetBlobClient(fileName);
        
        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }

}
