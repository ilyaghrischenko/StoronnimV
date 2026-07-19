using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Editing;

namespace StoronnimV.Application.Validation.GroupSocials;

public sealed class GroupSocialEditRequestValidator : AbstractValidator<GroupSocialEditRequest>
{
    public GroupSocialEditRequestValidator()
    {
        RuleFor(x => x.LinkUrl).MustBeExternalHttpUrl();
    }
}
