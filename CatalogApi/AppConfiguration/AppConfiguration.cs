using Microsoft.Extensions.Configuration;

namespace CatalogApi.AppConfiguration
{
    public interface IAppConfiguration
    {
        string GetConnectionString();
    }

    public class AppConfiguration : IAppConfiguration
    {
        private readonly IConfiguration _configuration;

        public AppConfiguration(IConfiguration configuration) => _configuration = configuration;

        public string GetConnectionString() => _configuration["APPSETTINGS:DBSETTINGS:CONNECTIONSTRING"];
    }
}
