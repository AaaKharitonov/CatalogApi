using System;
using Autofac;
using Autofac.Core;
using CatalogApi.AppConfiguration;
using CatalogApi.DAL;
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

            builder.RegisterGeneric(typeof(CatalogsRepository<>))
                .As(typeof(ICatalogsRepository<>))
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(DbContext) && pi.Name == "context",
                    (pi, ctx) => ctx.Resolve<DefaultDbContext>())
                );

            builder.Register(c =>
                {
                    var connectionString = c.Resolve<IAppConfiguration>().GetConnectionString();
                    return new Queries(connectionString, c.Resolve<EntitiesInfoService>());
                })
                .As<IQueries>()
                .SingleInstance();

            builder.Register(c => new EntitiesInfoService(c.Resolve<Func<DefaultDbContext>>()))
                .As<EntitiesInfoService>()
                .SingleInstance();

            builder.Register(c => new FakeEntitiesGeneratorService(c.Resolve<EntitiesInfoService>()))
                .As<FakeEntitiesGeneratorService>()
                .SingleInstance();
        }
    }
}
