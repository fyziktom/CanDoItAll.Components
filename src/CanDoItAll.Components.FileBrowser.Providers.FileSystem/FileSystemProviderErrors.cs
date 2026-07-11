using System.Security;
using CanDoItAll.Components.FileBrowser.Core;

namespace CanDoItAll.Components.FileBrowser.Providers.FileSystem;

/// <summary>Normalizes BCL filesystem failures without swallowing cooperative cancellation.</summary>
internal static class FileSystemProviderErrors
{
    public static T Execute<T>(Func<T> operation, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            return operation();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (FileBrowserProviderException)
        {
            throw;
        }
        catch (UnauthorizedAccessException exception)
        {
            throw Create(
                FileBrowserErrorCode.Forbidden,
                "Access to the requested filesystem location was denied.",
                technicalDetail: exception.Message,
                innerException: exception);
        }
        catch (SecurityException exception)
        {
            throw Create(
                FileBrowserErrorCode.Forbidden,
                "Access to the requested filesystem location was denied.",
                technicalDetail: exception.Message,
                innerException: exception);
        }
        catch (FileNotFoundException exception)
        {
            throw Create(
                FileBrowserErrorCode.NotFound,
                "The requested filesystem item no longer exists.",
                technicalDetail: exception.Message,
                innerException: exception);
        }
        catch (DirectoryNotFoundException exception)
        {
            throw Create(
                FileBrowserErrorCode.NotFound,
                "The requested filesystem directory no longer exists.",
                technicalDetail: exception.Message,
                innerException: exception);
        }
        catch (PathTooLongException exception)
        {
            throw Create(
                FileBrowserErrorCode.InvalidLocation,
                "The requested filesystem path is too long.",
                technicalDetail: exception.Message,
                innerException: exception);
        }
        catch (ArgumentException exception)
        {
            throw Create(
                FileBrowserErrorCode.InvalidLocation,
                "The requested filesystem location is invalid.",
                technicalDetail: exception.Message,
                innerException: exception);
        }
        catch (NotSupportedException exception)
        {
            throw Create(
                FileBrowserErrorCode.Unsupported,
                "The requested filesystem operation is not supported on this platform.",
                technicalDetail: exception.Message,
                innerException: exception);
        }
        catch (IOException exception)
        {
            throw Create(
                FileBrowserErrorCode.Unavailable,
                "The requested filesystem location is temporarily unavailable.",
                isRetryable: true,
                technicalDetail: exception.Message,
                innerException: exception);
        }
        catch (Exception exception)
        {
            throw Create(
                FileBrowserErrorCode.ProviderFailure,
                "The local filesystem provider could not complete the operation.",
                technicalDetail: exception.Message,
                innerException: exception);
        }
    }

    public static FileBrowserProviderException Create(
        FileBrowserErrorCode code,
        string message,
        bool isRetryable = false,
        string? technicalDetail = null,
        Exception? innerException = null)
        => new(
            new FileBrowserError(
                code,
                message,
                isRetryable,
                technicalDetail),
            innerException);
}
