using System.Runtime.InteropServices;

namespace CanDoItAll.Components.WebGlSandbox;

/// <summary>
/// Synthetic sample planes for the image view proof page: deterministic, generated on request, served as raw
/// little-endian bytes like any instrument or simulation output.
/// </summary>
public static class WebGlSandboxImages
{
    public const int Width = 320;
    public const int Height = 200;

    /// <summary>The raw bytes of a named plane, or null for an unknown name.</summary>
    public static byte[]? Bytes(string name) => name switch
    {
        "signal" => Floats(Signal(1.0)),
        "signal-brighter" => Floats(Signal(1.15)),
        "reference" => Floats(Reference()),
        "attenuated" => Floats(Attenuated()),
        "mask" => Mask(),
        "counts" => Counts(),
        _ => null,
    };

    // A smooth background with a bright spot, a dim ring, deterministic noise and a few non-finite samples.
    private static float[] Signal(double gain)
    {
        var values = new float[Width * Height];
        uint state = 2463534242;
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                double x = (column - Width * 0.62) / (Width * 0.12), y = (row - Height * 0.45) / (Height * 0.16);
                double r = Math.Sqrt(Math.Pow((column - Width * 0.3) / (Width * 0.16), 2) + Math.Pow((row - Height * 0.55) / (Height * 0.24), 2));
                double v = 120 + 0.25 * column + 900 * Math.Exp(-(x * x + y * y)) + 180 * Math.Exp(-Math.Pow((r - 1) / 0.12, 2));
                v += 18 * (Next(ref state) - 0.5);
                values[row * Width + column] = (float)(v * gain);
            }
        }

        values[17 * Width + 40] = float.NaN;
        values[120 * Width + 250] = float.PositiveInfinity;
        return values;
    }

    // An unobstructed field: flat with a gentle fall-off towards the corners.
    private static float[] Reference()
    {
        var values = new float[Width * Height];
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                double dx = (column - Width / 2.0) / Width, dy = (row - Height / 2.0) / Height;
                values[row * Width + column] = (float)(10000 * (1 - 0.35 * (dx * dx + dy * dy)));
            }
        }

        return values;
    }

    // The reference field behind a step wedge (five steps) and a disc: -ln(I / Iref) shows the thickness.
    private static float[] Attenuated()
    {
        float[] reference = Reference();
        var values = new float[Width * Height];
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                double thickness = Math.Min(4, column / (Width / 5)) * 0.25;
                double dx = column - Width * 0.7, dy = row - Height * 0.5;
                if (dx * dx + dy * dy < 900) thickness += 0.6;
                values[row * Width + column] = (float)(reference[row * Width + column] * Math.Exp(-thickness));
            }
        }

        return values;
    }

    // Invalid pixels: a two-pixel border and a short dead column.
    private static byte[] Mask()
    {
        var mask = new byte[Width * Height];
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                bool border = row < 2 || column < 2 || row >= Height - 2 || column >= Width - 2;
                bool dead = column == 211 && row is > 60 and < 140;
                mask[row * Width + column] = (byte)(border || dead ? 1 : 0);
            }
        }

        return mask;
    }

    // Unsigned 16-bit counts of the signal (a detector's raw counts).
    private static byte[] Counts()
    {
        float[] signal = Signal(1.0);
        var counts = new ushort[signal.Length];
        for (int i = 0; i < signal.Length; i++) counts[i] = (ushort)Math.Clamp(float.IsFinite(signal[i]) ? signal[i] * 20 : 0, 0, ushort.MaxValue);
        return MemoryMarshal.AsBytes(counts.AsSpan()).ToArray();
    }

    private static byte[] Floats(float[] values) => MemoryMarshal.AsBytes(values.AsSpan()).ToArray();

    private static double Next(ref uint state)
    {
        state ^= state << 13;
        state ^= state >> 17;
        state ^= state << 5;
        return state / (double)uint.MaxValue;
    }
}
