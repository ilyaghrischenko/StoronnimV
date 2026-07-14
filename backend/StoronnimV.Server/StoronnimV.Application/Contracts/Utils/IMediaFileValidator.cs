using Microsoft.AspNetCore.Http;
using StoronnimV.Application.Enums;

namespace StoronnimV.Application.Contracts.Utils;

public interface IMediaFileValidator
{
    Task ValidateAsync(IFormFile file, MediaKind mediaKind, CancellationToken ct);
}
