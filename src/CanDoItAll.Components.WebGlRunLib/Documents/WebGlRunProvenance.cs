namespace CanDoItAll.Components.WebGlRunLib;

public sealed class InputDocumentRef
{
    public string Kind { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class InputHashRef
{
    public string Kind { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class RunSourceRef
{
    public string SourceKind { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public List<InputDocumentRef> Inputs { get; set; } = [];
    public List<InputHashRef> Hashes { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunDocumentProvenanceValidationResult
{
    public List<string> Errors { get; set; } = [];
    public bool IsValid => Errors.Count == 0;
}

public sealed class WebGlRunDocumentProvenanceValidator
{
    private readonly string[] disallowedTerms;

    public WebGlRunDocumentProvenanceValidator()
        : this([])
    {
    }

    public WebGlRunDocumentProvenanceValidator(IEnumerable<string> disallowedTerms)
    {
        ArgumentNullException.ThrowIfNull(disallowedTerms);
        this.disallowedTerms = [.. disallowedTerms.Where(static term => !string.IsNullOrWhiteSpace(term)).Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    public WebGlRunDocumentProvenanceValidationResult Validate(WebGlRunDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var result = new WebGlRunDocumentProvenanceValidationResult();
        CheckMetadata("document", document.Metadata, result);
        return result;
    }

    public WebGlRunDocumentProvenanceValidationResult Validate(RunSourceRef source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var result = new WebGlRunDocumentProvenanceValidationResult();
        CheckValue("source.sourceKind", source.SourceKind, result);
        CheckValue("source.sourceId", source.SourceId, result);
        CheckMetadata("source.metadata", source.Metadata, result);

        foreach (InputDocumentRef input in source.Inputs)
        {
            CheckValue($"inputs.{input.Kind}.kind", input.Kind, result);
            CheckValue($"inputs.{input.Kind}.path", input.Path, result);
            CheckMetadata($"inputs.{input.Kind}.metadata", input.Metadata, result);
        }

        foreach (InputHashRef hash in source.Hashes)
        {
            CheckValue($"hashes.{hash.Kind}.kind", hash.Kind, result);
            CheckMetadata($"hashes.{hash.Kind}.metadata", hash.Metadata, result);
        }

        return result;
    }

    private void CheckMetadata(string scope, IReadOnlyDictionary<string, string> metadata, WebGlRunDocumentProvenanceValidationResult result)
    {
        foreach (KeyValuePair<string, string> item in metadata)
        {
            CheckValue($"{scope}.{item.Key}", item.Key, result);
            CheckValue($"{scope}.{item.Key}.value", item.Value, result);
        }
    }

    private void CheckValue(string path, string value, WebGlRunDocumentProvenanceValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        foreach (string term in disallowedTerms)
        {
            if (value.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add($"{path} contains domain-specific term '{term}'.");
            }
        }
    }
}
