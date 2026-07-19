using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

namespace StoronnimV.Application.Validation.Music;

public sealed class MusicPlatformAdditionRequestValidator : AbstractValidator<MusicPlatformAdditionRequest>
{
    public MusicPlatformAdditionRequestValidator()
    {
        RuleFor(x => x.PlatformUrl).MustBeExternalHttpUrl();
    }
}
