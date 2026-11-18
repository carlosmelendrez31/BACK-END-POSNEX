using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posnex.BLL.Businesses;
using Posnex.DAL.Businesses;
using Posnex.DAL.Persistence;

namespace Posnex.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Posnex");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Falta configurar ConnectionStrings:Posnex. Configúrala con User Secrets o con la variable ConnectionStrings__Posnex.");

        services.AddDbContext<PosnexDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IBusinessRepository, BusinessRepository>();
        return services;
    }
}
