namespace StoronnimV.Application.Exceptions;

public sealed class MediaConsistencyException(
    string containerName,
    string blobName,
    Exception innerException)
    : Exception($"Media compensation failed for '{containerName}/{blobName}'.", innerException)
{
    public string ContainerName { get; } = containerName;
    public string BlobName { get; } = blobName;
}
