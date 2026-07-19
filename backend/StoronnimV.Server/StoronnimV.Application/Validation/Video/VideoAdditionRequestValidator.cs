using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;
using StoronnimV.Domain.Enums;

namespace StoronnimV.Application.Validation.Video;

public sealed class VideoAdditionRequestValidator : AbstractValidator<VideoAdditionRequest>
{
    public VideoAdditionRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(type => Enum.TryParse<VideoType>(type, out _))
            .WithMessage("Invalid video type");
    }
}
