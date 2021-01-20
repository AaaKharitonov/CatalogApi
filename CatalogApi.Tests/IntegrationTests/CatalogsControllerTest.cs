using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

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
            _client = new HttpClient();
        }

        [Theory]
        [InlineData("simple")]
        [InlineData("1")]
        [InlineData("2")]
        public async Task Test(string route)
        {
            _output.WriteLine($"Test2 val: {route}");

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"{_applicationUrl}/{route}");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                _output.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                _output.WriteLine("Exception Caught!");
                _output.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}
