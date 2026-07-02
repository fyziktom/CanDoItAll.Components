using CanDoItAll.Components.QRCode.Models;

namespace CanDoItAll.Components.QRCode;

public interface IQrCodeRenderer
{
    string RenderSvg(string payload, QrCodeRenderOptions? options = null);
}
