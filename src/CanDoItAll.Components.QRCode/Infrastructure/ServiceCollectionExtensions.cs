using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CanDoItAll.Components.QRCode;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCanDoItAllQrCode(this IServiceCollection services)
    {
        services.TryAddScoped<IQrCodeRenderer, NetCodecreteQrCodeRenderer>();
        return services;
    }
}
