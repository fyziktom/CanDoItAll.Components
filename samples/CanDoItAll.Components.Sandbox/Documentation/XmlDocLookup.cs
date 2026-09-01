using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CanDoItAll.Components.Sandbox.Documentation;

/// <summary>
/// Reads <c>&lt;summary&gt;</c> text out of the XML documentation file that the compiler emits next
/// to each referenced assembly.
/// </summary>
/// <remarks>
/// <c>///</c> comments written inside a Razor <c>@code</c> block do reach this file, so the component
/// source stays the single place a parameter is described. The file is a sibling of the assembly
/// rather than an embedded resource, so a static WASM host would not have it — every lookup therefore
/// degrades to <c>null</c> instead of throwing.
/// </remarks>
public static partial class XmlDocLookup
{
    private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> Cache =
        new(StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, string> Empty =
        new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>
    /// Returns the summary for a type, or <c>null</c> when it is undocumented.
    /// </summary>
    public static string? ForType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Lookup(type.Assembly, $"T:{TypeKey(type)}");
    }

    /// <summary>
    /// Returns the summary for a property, or <c>null</c> when it is undocumented.
    /// </summary>
    public static string? ForProperty(PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(property);

        var declaringType = property.DeclaringType;
        if (declaringType is null)
        {
            return null;
        }

        return Lookup(declaringType.Assembly, $"P:{TypeKey(declaringType)}.{property.Name}");
    }

    private static string? Lookup(Assembly assembly, string memberKey)
    {
        var members = MembersFor(assembly);
        return members.TryGetValue(memberKey, out var summary) ? summary : null;
    }

    private static IReadOnlyDictionary<string, string> MembersFor(Assembly assembly)
    {
        var name = assembly.GetName().Name;
        if (string.IsNullOrEmpty(name))
        {
            return Empty;
        }

        return Cache.GetOrAdd(name, static key => Load(key));
    }

    private static IReadOnlyDictionary<string, string> Load(string assemblyName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");

        try
        {
            if (!File.Exists(path))
            {
                return Empty;
            }

            var document = XDocument.Load(path);

            return document
                .Descendants("member")
                .Select(static member => new
                {
                    Name = member.Attribute("name")?.Value,
                    Summary = Flatten(member.Element("summary"))
                })
                .Where(static entry => !string.IsNullOrEmpty(entry.Name) && !string.IsNullOrEmpty(entry.Summary))
                .GroupBy(static entry => entry.Name!, StringComparer.Ordinal)
                .ToDictionary(
                    static group => group.Key,
                    static group => group.First().Summary!,
                    StringComparer.Ordinal);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PlatformNotSupportedException or System.Xml.XmlException)
        {
            // Documentation is a convenience here; never let a missing or malformed file break the page.
            return Empty;
        }
    }

    /// <summary>
    /// Collapses an XML summary into a single line, rendering the common inline tags as plain text.
    /// </summary>
    private static string? Flatten(XElement? summary)
    {
        if (summary is null)
        {
            return null;
        }

        var builder = new StringBuilder();
        Append(summary, builder);

        var text = string.Join(' ', builder.ToString().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

        // Inline tags are padded so they never fuse with neighbouring words; that padding leaves a
        // stray space when the tag is followed by punctuation ("see ButtonStyle ." -> "see ButtonStyle.").
        text = SpaceBeforePunctuation().Replace(text, "$1");

        return text.Length == 0 ? null : text;
    }

    private static void Append(XNode node, StringBuilder builder)
    {
        switch (node)
        {
            case XText text:
                builder.Append(text.Value);
                break;

            case XElement element:
                switch (element.Name.LocalName)
                {
                    case "see":
                    case "seealso":
                    case "paramref":
                    case "typeparamref":
                        builder.Append(' ').Append(CrefText(element)).Append(' ');
                        break;

                    case "para":
                        builder.Append(' ');
                        foreach (var child in element.Nodes())
                        {
                            Append(child, builder);
                        }

                        builder.Append(' ');
                        break;

                    default:
                        foreach (var child in element.Nodes())
                        {
                            Append(child, builder);
                        }

                        break;
                }

                break;
        }
    }

    private static string CrefText(XElement element)
    {
        var reference = element.Attribute("cref")?.Value
            ?? element.Attribute("name")?.Value
            ?? element.Value;

        if (string.IsNullOrWhiteSpace(reference))
        {
            return string.Empty;
        }

        // Strip the "T:"/"P:"/"M:" prefix and the namespace, leaving the readable tail.
        var trimmed = reference.Length > 2 && reference[1] == ':' ? reference[2..] : reference;
        var lastDot = trimmed.LastIndexOf('.');
        return lastDot >= 0 && lastDot < trimmed.Length - 1 ? trimmed[(lastDot + 1)..] : trimmed;
    }

    [GeneratedRegex(@"\s+([.,;:!?)\]])")]
    private static partial Regex SpaceBeforePunctuation();

    private static string TypeKey(Type type)
    {
        // Documentation ids use the open generic arity form, e.g. Namespace.Grid`1.
        return type.IsConstructedGenericType
            ? type.GetGenericTypeDefinition().FullName ?? type.Name
            : type.FullName ?? type.Name;
    }
}
