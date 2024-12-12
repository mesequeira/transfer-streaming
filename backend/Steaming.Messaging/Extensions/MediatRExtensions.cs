using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Steaming.Messaging.Extensions;

public static class MediatRExtensions
{
    public static void AddMediatRConfiguration(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(configurator =>
        {
            configurator.RegisterServicesFromAssembly(assembly);
            
        });
    }
}