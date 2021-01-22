using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CatalogApi.Domain.Catalogs;
using CatalogApi.Tests.Utilities;
using Xunit;
using Xunit.Abstractions;

//HttpClient - for CatalogsController

// 1. Run api in vs in Debug mode. 
// 2. Resharper Unit Test Session -> Options -> Build Policy: Never 
// 3. Run tests and Debug 

namespace CatalogApi.Tests.IntegrationTests
{
    public class CatalogsControllerTest
    {
        private readonly ITestOutputHelper _output;
        private readonly string _applicationUrl;
        private readonly HttpClient _client;

        public CatalogsControllerTest(ITestOutputHelper output)
        {
            _output = output;
            _applicationUrl = "http://localhost:5000";
            _client = new HttpClient(new HttpClientMessageHandler(new HttpClientHandler(), output));
        }

        [Theory]
        [InlineData("simple")]
        [InlineData("not_existing_route")]
        public async Task GetAll(string route)
        {
            var url = $"{_applicationUrl}/{route}";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                _output.WriteLine($"Exception message :{e.Message}");
            }
        }

        [Theory]
        [InlineData("simple", 1)]
        [InlineData("simple", 2)]
        [InlineData("not_existing_route", 1)]
        public async Task Get(string route, int id)
        {
            var url = $"{_applicationUrl}/{route}/{id}";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                _output.WriteLine($"Exception message :{e.Message}");
            }
        }

        [Theory]
        [InlineData("simple")]
        [InlineData("not_existing_route")]
        public async Task Post(string route)
        {
            var url = $"{_applicationUrl}/{route}";

            try
            {
                SimpleCatalog simpleCatalog = new SimpleCatalog
                {
                    Id = 2,
                    Name = "Name2",
                    Val = 200
                };

                HttpResponseMessage response = await _client.PostAsJsonAsync(url, simpleCatalog);

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                _output.WriteLine($"Exception message :{e.Message}");
            }
        }

        [Theory]
        [InlineData("simple", 1)]
        [InlineData("simple", 2)]
        [InlineData("not_existing_route", 1)]
        public async Task Put(string route, int id)
        {
            var url = $"{_applicationUrl}/{route}/{id}";

            SimpleCatalog simpleCatalog = new SimpleCatalog
            {
                Id = 2,
                Name = "NewName",
                Val = 200000
            };

            try
            {
                HttpResponseMessage response = await _client.PutAsJsonAsync(url, simpleCatalog);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                _output.WriteLine($"Exception message :{e.Message}");
            }
        }

        [Theory]
        [InlineData("simple", 1)]
        [InlineData("simple", 2)]
        [InlineData("not_existing_route", 1)]
        public async Task Delete(string route, int id)
        {
            var url = $"{_applicationUrl}/{route}/{id}";

            try
            {
                HttpResponseMessage response = await _client.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                _output.WriteLine($"Exception message :{e.Message}");
            }
        }
    }
}
