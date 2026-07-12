using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Admin;

namespace StoronnimV.Application.Validation.Admin;

public class EditBasicAdminLoginRequestValidator : AbstractValidator<EditBasicAdminLoginRequest>
{
    public EditBasicAdminLoginRequestValidator()
    {
        RuleFor(x => x.NewLogin)
            .NotEmpty().WithMessage("Login is required")
            .MinimumLength(4).WithMessage("Login must be at least 4 characters long")
            .Matches("[A-Za-z]").WithMessage("Login must contain at least one letter")
            .Matches("[0-9]").WithMessage("Login must contain at least one number");
    }
}