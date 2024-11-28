using System.Reflection;

namespace StoronnimV.Application.Extensions;

/// <summary>
/// Методы расширения для object
/// </summary>
public static class ObjectExtensions
{
    public static object? GetPropertyValue(this object? obj, string propertyName)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        return property.GetValue(obj);
    }
}