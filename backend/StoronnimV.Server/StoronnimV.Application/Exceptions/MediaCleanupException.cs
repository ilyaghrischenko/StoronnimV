namespace StoronnimV.Application.Exceptions;

public sealed class MediaCleanupException(
    string containerName,
    string blobName,
    Exception innerException)
    : Exception($"Media cleanup is required for '{containerName}/{blobName}'.", innerException)
{
    public string ContainerName { get; } = containerName;
    public string BlobName { get; } = blobName;
}
