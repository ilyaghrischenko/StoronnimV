namespace StoronnimV.Domain.Contracts.AzureBlobStorage;

public interface IBlobRepository
{
    Task<string> AddFileAndGetUrlAsync(string containerName, string fileName, Stream fileStream, CancellationToken ct);
    string GetFileUrl(string containerName, string fileName, CancellationToken ct);
    Task DeleteFileAsync(string containerName, string fileName, CancellationToken ct);
    Task DeleteAllFilesByNameAsync(string containerName, string fileName, CancellationToken ct);
}