using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using CatalogApi.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogApi
{
    public static class HostExtensions
    {
        public static IHost InitEntitiesInfoService(this IHost host)
        {
            var entitiesInfoService = (EntitiesInfoService)host.Services.GetService(typeof(EntitiesInfoService));

            entitiesInfoService?.Init();

            return host;
        }

        public static IHost MigrateDbContext<TContext>(this IHost webHost) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                scope.ServiceProvider
                    .GetService<TContext>()
                    .Database
                    .Migrate();
            }

            return webHost;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDbContext<DefaultDbContext>()
                .InitEntitiesInfoService()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
