using System.Reflection;
using StoronnimV.Application.Exceptions;

namespace StoronnimV.Application.Extensions;

/// <summary>
/// Метод расширения для object
/// </summary>
public static class ObjectExtensions
{
    public static object? GetPropertyValue(this object? obj, string propertyName)
    {
        if (obj is null)
        {
            throw new GetPropertyValueException($"Object: {obj} is null");
        }

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new GetPropertyValueException($"Property: {propertyName} in object: {obj} is null or empty");
        }

        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property is null)
        {
            throw new GetPropertyValueException($"Property: {propertyName} in object: {obj} after getting is null");
        }

        return property.GetValue(obj);
    }
}