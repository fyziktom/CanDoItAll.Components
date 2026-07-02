using CanDoItAll.Components.QRCode.Models;
using Net.Codecrete.QrCodeGenerator;

namespace CanDoItAll.Components.QRCode;

public sealed class NetCodecreteQrCodeRenderer : IQrCodeRenderer
{
    public string RenderSvg(string payload, QrCodeRenderOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("QR payload cannot be empty.", nameof(payload));
        }

        var resolvedOptions = options ?? new QrCodeRenderOptions();
        var qr = QrCode.EncodeText(payload, MapErrorCorrection(resolvedOptions.ErrorCorrectionLevel));
        return qr.ToSvgString(Math.Max(0, resolvedOptions.BorderModules));
    }

    private static QrCode.Ecc MapErrorCorrection(QrErrorCorrectionLevel level)
        => level switch
        {
            QrErrorCorrectionLevel.Low => QrCode.Ecc.Low,
            QrErrorCorrectionLevel.Quartile => QrCode.Ecc.Quartile,
            QrErrorCorrectionLevel.High => QrCode.Ecc.High,
            _ => QrCode.Ecc.Medium
        };
}
