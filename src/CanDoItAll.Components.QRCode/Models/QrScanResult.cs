namespace CanDoItAll.Components.QRCode.Models;

public sealed record QrScanResult(
    string Text,
    QrPayloadKind Kind,
    bool IsValid,
    string Source,
    string? Error = null)
{
    public static QrScanResult Invalid(string text, string source, string error)
        => new(text, QrPayloadKind.Generic, false, source, error);
}
