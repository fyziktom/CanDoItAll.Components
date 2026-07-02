namespace CanDoItAll.Components.QRCode.Models;

public sealed record QrCodeRenderOptions
{
    public int BorderModules { get; init; } = 4;

    public QrErrorCorrectionLevel ErrorCorrectionLevel { get; init; } = QrErrorCorrectionLevel.Medium;
}
