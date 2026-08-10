using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox.Documentation;

/// <summary>
/// One row of a component API table.
/// </summary>
/// <param name="Name">Parameter name as written at the call site.</param>
/// <param name="TypeLabel">Readable C# type, for example <c>EventCallback&lt;string&gt;</c>.</param>
/// <param name="DefaultValue">Default read off a fresh instance, or <c>null</c> when unavailable.</param>
/// <param name="IsRequired">True when the parameter carries <see cref="EditorRequiredAttribute"/>.</param>
/// <param name="IsCaptureUnmatched">True for the splatted attribute dictionary.</param>
/// <param name="IsInherited">True when the parameter comes from a base class rather than the component itself.</param>
/// <param name="EnumValues">Members of the parameter's enum type, empty for non-enums.</param>
/// <param name="Description">Text from the <c>///</c> comment, or <c>null</c> when undocumented.</param>
public sealed record ComponentParameterInfo(
    string Name,
    string TypeLabel,
    string? DefaultValue,
    bool IsRequired,
    bool IsCaptureUnmatched,
    bool IsInherited,
    IReadOnlyList<string> EnumValues,
    string? Description);

/// <summary>
/// A component's public parameter surface, as reflected from the compiled type.
/// </summary>
public sealed record ComponentApiInfo(
    Type Type,
    string Name,
    string? Description,
    IReadOnlyList<ComponentParameterInfo> Parameters)
{
    /// <summary>Number of parameters that still have no <c>///</c> comment.</summary>
    public int UndocumentedCount => Parameters.Count(static parameter => string.IsNullOrEmpty(parameter.Description));
}

/// <summary>
/// Builds <see cref="ComponentApiInfo"/> by reflecting over a component type.
/// </summary>
/// <remarks>
/// Parameter names, types, defaults, enum members and required-ness all come from the compiled
/// assembly, so they cannot drift from the implementation. Only the prose comes from a second place —
/// the XML documentation file, which is itself generated from the component source.
/// </remarks>
public static class ComponentApiReflector
{
    private static readonly ConcurrentDictionary<Type, ComponentApiInfo> Cache = new();

    /// <summary>
    /// Describes a component type. Results are cached for the lifetime of the process.
    /// </summary>
    public static ComponentApiInfo Describe(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Cache.GetOrAdd(type, static key => Build(key));
    }

    /// <summary>
    /// Describes several component types in the order supplied.
    /// </summary>
    public static IReadOnlyList<ComponentApiInfo> Describe(IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return types.Select(Describe).ToArray();
    }

    private static ComponentApiInfo Build(Type type)
    {
        var instance = TryCreateInstance(type);

        var parameters = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => new
            {
                Property = property,
                Attribute = property.GetCustomAttribute<ParameterAttribute>()
            })
            .Where(static entry => entry.Attribute is not null)
            .Select(entry => Describe(entry.Property, entry.Attribute!, type, instance))
            .OrderBy(static parameter => parameter.IsInherited)
            .ThenByDescending(static parameter => parameter.IsRequired)
            .ThenBy(static parameter => parameter.Name, StringComparer.Ordinal)
            .ToArray();

        return new ComponentApiInfo(
            type,
            FriendlyTypeName(type),
            XmlDocLookup.ForType(type),
            parameters);
    }

    private static ComponentParameterInfo Describe(
        PropertyInfo property,
        ParameterAttribute attribute,
        Type componentType,
        object? instance)
    {
        var enumType = UnwrapNullable(property.PropertyType);

        return new ComponentParameterInfo(
            property.Name,
            FriendlyTypeName(property.PropertyType),
            FormatDefault(property, instance),
            property.GetCustomAttribute<EditorRequiredAttribute>() is not null,
            attribute.CaptureUnmatchedValues,
            property.DeclaringType != componentType,
            enumType.IsEnum ? Enum.GetNames(enumType) : [],
            XmlDocLookup.ForProperty(property));
    }

    private static object? TryCreateInstance(Type type)
    {
        try
        {
            // Defaults are only readable from a real instance. Components that expect a cascading
            // value or an injected service can throw here; a missing default must not take the
            // whole table down.
            return Activator.CreateInstance(type);
        }
        catch (Exception exception) when (exception is MissingMethodException or MemberAccessException or TargetInvocationException or NotSupportedException)
        {
            return null;
        }
    }

    private static string? FormatDefault(PropertyInfo property, object? instance)
    {
        if (instance is null || !property.CanRead)
        {
            return null;
        }

        // An unset callback has no meaningful default, and EventCallback<T>.ToString() falls back to
        // the raw CLR type name, which is both wrong and wide enough to crowd out the description.
        if (IsEventCallback(property.PropertyType))
        {
            return null;
        }

        object? value;
        try
        {
            value = property.GetValue(instance);
        }
        catch (Exception exception) when (exception is TargetInvocationException or MethodAccessException or NotSupportedException)
        {
            return null;
        }

        return value switch
        {
            null => null,
            string text when text.Length == 0 => "\"\"",
            string text => $"\"{text}\"",
            bool flag => flag ? "true" : "false",
            Enum enumValue => $"{FriendlyTypeName(enumValue.GetType())}.{enumValue}",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => Shorten(value.ToString())
        };
    }

    /// <summary>
    /// Guards the table against a type whose <c>ToString</c> returns an assembly-qualified name.
    /// </summary>
    private static string? Shorten(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return text.Length <= 40 ? text : $"{text[..39]}…";
    }

    private static bool IsEventCallback(Type type)
    {
        return type == typeof(EventCallback)
            || (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(EventCallback<>));
    }

    private static Type UnwrapNullable(Type type) => Nullable.GetUnderlyingType(type) ?? type;

    /// <summary>
    /// Renders a type the way it would be written in C#, including nullability and generic arguments.
    /// </summary>
    internal static string FriendlyTypeName(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is not null)
        {
            return $"{FriendlyTypeName(underlying)}?";
        }

        if (type.IsArray)
        {
            return $"{FriendlyTypeName(type.GetElementType()!)}[]";
        }

        if (type.IsConstructedGenericType)
        {
            var definition = type.GetGenericTypeDefinition().Name;
            var tick = definition.IndexOf('`', StringComparison.Ordinal);
            var name = tick > 0 ? definition[..tick] : definition;
            var arguments = string.Join(", ", type.GetGenericArguments().Select(FriendlyTypeName));
            return $"{name}<{arguments}>";
        }

        return Aliases.TryGetValue(type, out var alias) ? alias : type.Name;
    }

    private static readonly Dictionary<Type, string> Aliases = new()
    {
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(char)] = "char",
        [typeof(decimal)] = "decimal",
        [typeof(double)] = "double",
        [typeof(float)] = "float",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(object)] = "object",
        [typeof(string)] = "string"
    };
}
