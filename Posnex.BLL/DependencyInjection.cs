using Microsoft.Extensions.DependencyInjection;
using Posnex.BLL.Businesses;

namespace Posnex.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IBusinessService, BusinessService>();
        return services;
    }
}
