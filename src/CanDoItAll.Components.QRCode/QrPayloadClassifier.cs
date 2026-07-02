using CanDoItAll.Components.QRCode.Models;

namespace CanDoItAll.Components.QRCode;

public static class QrPayloadClassifier
{
    public static QrPayloadKind Classify(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return QrPayloadKind.Generic;
        }

        var value = payload.Trim();
        if (value.StartsWith("candoitall-ledger:address:", StringComparison.OrdinalIgnoreCase))
        {
            return QrPayloadKind.Address;
        }

        if (value.StartsWith("candoitall-ledger:subaddress:", StringComparison.OrdinalIgnoreCase))
        {
            return QrPayloadKind.Subaddress;
        }

        if (value.StartsWith("candoitall-ledger:tx:", StringComparison.OrdinalIgnoreCase))
        {
            return QrPayloadKind.TransactionHash;
        }

        if (value.StartsWith("candoitall-ledger:ticket-ownership:", StringComparison.OrdinalIgnoreCase))
        {
            return QrPayloadKind.TicketOwnershipHash;
        }

        if (value.StartsWith("candoitall-ledger:bo-ownership:", StringComparison.OrdinalIgnoreCase))
        {
            return QrPayloadKind.BusinessObjectOwnershipHash;
        }

        if (value.StartsWith("cdia", StringComparison.OrdinalIgnoreCase))
        {
            return QrPayloadKind.Address;
        }

        if (IsHexHash(value))
        {
            return QrPayloadKind.TransactionHash;
        }

        return QrPayloadKind.Generic;
    }

    public static QrScanResult BuildScanResult(string? payload, string source)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return QrScanResult.Invalid(string.Empty, source, "The scanned payload is empty.");
        }

        var text = payload.Trim();
        return new QrScanResult(text, Classify(text), true, source);
    }

    private static bool IsHexHash(string value)
    {
        if (value.Length is < 32 or > 128)
        {
            return false;
        }

        return value.All(static c =>
            c is >= '0' and <= '9'
            || c is >= 'a' and <= 'f'
            || c is >= 'A' and <= 'F');
    }
}
