using System;
using Autofac;
using CatalogApi.AppConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CatalogApi.AutofacModules
{
    public class MainModule : Module
    {
        private readonly IConfiguration _configuration;

        public MainModule(IConfiguration configuration) => _configuration = configuration;

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new AppConfiguration.AppConfiguration(_configuration))
                .As<IAppConfiguration>()
                .SingleInstance();

            builder.Register(c =>
                {
                    var connectionString = c.Resolve<IAppConfiguration>().GetConnectionString();
                    var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>()
                        .UseNpgsql(connectionString,
                            options => { options.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null); });

                    return new DefaultDbContext(optionsBuilder.Options);
                })
                .InstancePerLifetimeScope();

            //DefaultDbContext factory
            builder.Register<Func<DefaultDbContext>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return () =>
                {
                    var connectionString = context.Resolve<IAppConfiguration>().GetConnectionString();
                    var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>()
                        .UseNpgsql(connectionString,
                            options => { options.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null); });

                    return new DefaultDbContext(optionsBuilder.Options);
                };
            }).SingleInstance();
        }
    }
}
