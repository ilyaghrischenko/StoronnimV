using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Pages.Addition;

namespace StoronnimV.Application.Validation.GroupSocials;

public sealed class GroupSocialAdditionRequestValidator : AbstractValidator<GroupSocialAdditionRequest>
{
    public GroupSocialAdditionRequestValidator()
    {
        RuleFor(x => x.LinkUrl).MustBeExternalHttpUrl();
    }
}
