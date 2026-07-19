using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;
using StoronnimV.Domain.Enums;

namespace StoronnimV.Application.Validation.Video;

public sealed class VideoEditRequestValidator : AbstractValidator<VideoEditRequest>
{
    public VideoEditRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(type => Enum.TryParse<VideoType>(type, out _))
            .WithMessage("Invalid video type");
    }
}
