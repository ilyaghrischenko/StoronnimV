using DotNetEnv;

namespace StoronnimV.Api.Extensions;

public static class EnvironmentExtensions
{
    public static string GetEnvironmentVariableOrThrowException(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);

        if (value is null)
        {
            throw new EnvVariableNotFoundException("Environment variable not found: " + key, key);
        }

        return value;
    }
}