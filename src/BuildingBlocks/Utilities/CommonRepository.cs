using System.Reflection;

namespace BuildingBlocks.Utilities;

public static class CommonRepository
{
    public static bool IsValidProperty(string propertyName, Type type)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return false;
        }

        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        
        if (property == null)
        {
            return false; // Property does not exist
        }

        // Check if the property type is a simple type
        return IsSimpleType(property.PropertyType);
    }
    
    public static bool IsSimpleType(Type type)
    {
        // Simple types include primitives, enums, strings, and DateTime
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(Guid) ||
               (type.IsValueType && !type.IsGenericType); // Structs without generics
    }
}