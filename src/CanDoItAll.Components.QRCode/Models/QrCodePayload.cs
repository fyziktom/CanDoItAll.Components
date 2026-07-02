namespace CanDoItAll.Components.QRCode.Models;

public sealed record QrCodePayload(
    string Text,
    QrPayloadKind Kind = QrPayloadKind.Generic,
    string? Label = null,
    string? Detail = null)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Text);

    public string DisplayLabel => string.IsNullOrWhiteSpace(Label)
        ? Kind.ToString()
        : Label;
}
