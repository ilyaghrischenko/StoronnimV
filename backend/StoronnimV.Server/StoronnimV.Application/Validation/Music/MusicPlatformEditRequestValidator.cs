using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;

namespace StoronnimV.Application.Validation.Music;

public sealed class MusicPlatformEditRequestValidator : AbstractValidator<MusicPlatformEditRequest>
{
    public MusicPlatformEditRequestValidator()
    {
        RuleFor(x => x.PlatformUrl).MustBeExternalHttpUrl();
    }
}
