namespace CanDoItAll.Components.WebGlLib;

public readonly record struct WebGlVector3(double X, double Y, double Z)
{
    public static WebGlVector3 Zero { get; } = new(0, 0, 0);

    public static WebGlVector3 One { get; } = new(1, 1, 1);
}

