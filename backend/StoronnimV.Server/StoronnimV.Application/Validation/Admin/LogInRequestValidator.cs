using FluentValidation;
using StoronnimV.Application.DTO.Requests.Account;

namespace StoronnimV.Application.Validation.Admin;

public class LogInRequestValidator : AbstractValidator<LogInRequest>
{
    public LogInRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login field is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password field is required");
    }
}