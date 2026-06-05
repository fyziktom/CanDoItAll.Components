namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunDomainMappingDriver
{
    string DriverId { get; }
    string DisplayName { get; }
    WebGlRunGenericBoundaryOptions BoundaryOptions { get; }
    IReadOnlyCollection<string> DriverActionKinds { get; }
    string MapToGenericActionKind(string driverActionKind);
}

public sealed class WebGlRunPassThroughDomainMappingDriver : IWebGlRunDomainMappingDriver
{
    public static WebGlRunPassThroughDomainMappingDriver Instance { get; } = new();

    public string DriverId => "generic-pass-through";
    public string DisplayName => "Generic pass-through";
    public WebGlRunGenericBoundaryOptions BoundaryOptions => WebGlRunGenericBoundaryOptions.None;
    public IReadOnlyCollection<string> DriverActionKinds => [];

    public string MapToGenericActionKind(string driverActionKind)
        => string.IsNullOrWhiteSpace(driverActionKind) ? WebGlRunActionKinds.Wait : driverActionKind;
}
