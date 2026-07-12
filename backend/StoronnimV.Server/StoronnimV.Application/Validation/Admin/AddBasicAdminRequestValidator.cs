using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Admin;

namespace StoronnimV.Application.Validation.Admin;

public sealed class AddBasicAdminRequestValidator : AbstractValidator<CreateBasicAdminRequest>
{
    public AddBasicAdminRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required")
            .MinimumLength(4).WithMessage("Login is required")
            .Matches("[A-Za-z]").WithMessage("Login must contain at least one letter")
            .Matches("[0-9]").WithMessage("Login must contain at least one number");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(10).WithMessage("Password must be at least 10 characters")
            .MaximumLength(15).WithMessage("Password must be between 10 and 15 characters")
            .Matches("^(?=(.*[a-zA-Z]){5,})(?=(.*[A-Z]){3,})").WithMessage("Password must contain at least 5 letters, including at least 3 uppercase letters")
            .Matches(@"(\d.*){5}").WithMessage("Password must contain at least 5 digits");
    }
}