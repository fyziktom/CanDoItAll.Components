using CanDoItAll.Components.QRCode.Models;

namespace CanDoItAll.Components.QRCode.Tests;

public sealed class QrCodeRendererTests
{
    [Fact]
    public void SB01_QrRendererRejectsEmptyPayload()
    {
        var renderer = new NetCodecreteQrCodeRenderer();

        Assert.Throws<ArgumentException>(() => renderer.RenderSvg(" "));
    }

    [Fact]
    public void SB01_QrRendererProducesSvgForAddressPayload()
    {
        var renderer = new NetCodecreteQrCodeRenderer();

        string svg = renderer.RenderSvg("candoitall-ledger:address:cdia1Ngb3miLzYorvFSfKzLFwYPvZH6Kw6LPsSu");

        Assert.Contains("<svg", svg, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("</svg>", svg, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("path", svg, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("candoitall-ledger:address:cdia1abc", QrPayloadKind.Address)]
    [InlineData("candoitall-ledger:subaddress:cdia1sub", QrPayloadKind.Subaddress)]
    [InlineData("candoitall-ledger:tx:29c3d5d793b3fa", QrPayloadKind.TransactionHash)]
    [InlineData("candoitall-ledger:ticket-ownership:hash", QrPayloadKind.TicketOwnershipHash)]
    [InlineData("candoitall-ledger:bo-ownership:hash", QrPayloadKind.BusinessObjectOwnershipHash)]
    [InlineData("plain text", QrPayloadKind.Generic)]
    public void SB01_QrPayloadClassifierSeparatesLedgerPayloadKinds(string payload, QrPayloadKind expected)
    {
        Assert.Equal(expected, QrPayloadClassifier.Classify(payload));
    }

    [Fact]
    public void SB01_QrScanResultRejectsBlankManualPayload()
    {
        var result = QrPayloadClassifier.BuildScanResult(" ", "manual");

        Assert.False(result.IsValid);
        Assert.Equal("manual", result.Source);
        Assert.NotNull(result.Error);
    }
}
