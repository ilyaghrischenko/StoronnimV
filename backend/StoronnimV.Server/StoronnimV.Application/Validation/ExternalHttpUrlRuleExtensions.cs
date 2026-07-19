using FluentValidation;

namespace StoronnimV.Application.Validation;

internal static class ExternalHttpUrlRuleExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeExternalHttpUrl<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .Must(IsValidExternalHttpUrl)
            .WithMessage("'{PropertyName}' must be an absolute HTTP or HTTPS URL without credentials.");
    }

    private static bool IsValidExternalHttpUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value != value.Trim() ||
            !Uri.TryCreate(value, UriKind.Absolute, out Uri? uri))
        {
            return false;
        }

        return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
               !string.IsNullOrWhiteSpace(uri.Host) &&
               string.IsNullOrEmpty(uri.UserInfo);
    }
}
