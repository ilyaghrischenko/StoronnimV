using FluentValidation;
using StoronnimV.Application.DTO.Requests.Entities.Admin;

namespace StoronnimV.Application.Validation.Admin;

public class EditBasicAdminPasswordRequestValidator : AbstractValidator<EditBasicAdminPasswordRequest>
{
    public EditBasicAdminPasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Old password is required")
            .MinimumLength(10).WithMessage("Old password must be at least 10 characters")
            .MaximumLength(15).WithMessage("Old password must be between 10 and 15 characters")
            .Matches("^(?=(.*[a-zA-Z]){5,})(?=(.*[A-Z]){3,})").WithMessage("Old password must contain at least 5 letters, including at least 3 uppercase letters")
            .Matches(@"(\d.*){5}").WithMessage("Old password must contain at least 5 digits");
        
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(10).WithMessage("New password must be at least 10 characters")
            .MaximumLength(15).WithMessage("New password must be between 10 and 15 characters")
            .Matches("^(?=(.*[a-zA-Z]){5,})(?=(.*[A-Z]){3,})").WithMessage("New password must contain at least 5 letters, including at least 3 uppercase letters")
            .Matches(@"(\d.*){5}").WithMessage("New password must contain at least 5 digits")
            .Must((request, newPassword) => newPassword != request.OldPassword)
            .WithMessage("New password must not be the same as the old password");
    }
}