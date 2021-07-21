using Microsoft.Extensions.DependencyInjection;
using Model.Capabilities.Validation;
using Model.Repositories;
using Model.Services;
using Model.Services.Interfaces;
using Persistence.Repositories;

namespace ServiceHost.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static void ConfigureModelServices(this IServiceCollection services)
        {
            services.AddScoped<IProductCatalogService, ProductCatalogService>();
            services.AddSingleton(new ValidatorFactory());
        }

        public static void ConfigurePersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<IProductCatalogRepository, DBProductCatalogRepository>();
        }
    }
}
