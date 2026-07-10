# CanDoItAll.Components.QRCode

QRCode provides Blazor building blocks for showing and scanning QR codes: SVG rendering, copy-friendly dialog content, page-header actions, a scanner shell, and typed scan results. It stays payload-neutral, so your application decides what a QR code represents and how to validate or act on it.

## Quick start

Register the renderer, make the dialog host available through BaseLib, then render a payload:

```csharp
// Program.cs
builder.Services.AddCanDoItAllBaseLib();
builder.Services.AddCanDoItAllQrCode();
```

```razor
@using CanDoItAll.Components.QRCode
@using CanDoItAll.Components.QRCode.Components

<QrCodeView Payload="https://example.test/invite/7K3D"
            Caption="Scan to open the invitation."
            AriaLabel="Invitation QR code" />
```

Use `QrCodeButton` to open a QR dialog from a toolbar or page header. Use `QrScanButton` and its `Scanned` callback to receive a `QrScanResult`; preserve a manual-text path when camera access is unavailable. The Sandbox `/groups/qr` route shows the expected host setup.

The project owns generic QR UI only. Address, ticket, transaction, business-object, and other payload semantics belong in the consuming application or domain component library.
