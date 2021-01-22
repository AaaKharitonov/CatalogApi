using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CatalogApi.Tests.Utilities
{
    public class HttpClientMessageHandler : DelegatingHandler
    {
        private readonly ITestOutputHelper _output;

        public HttpClientMessageHandler(HttpMessageHandler innerHandler, ITestOutputHelper output)
            : base(innerHandler) => _output = output;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _output.WriteLine($"Request: \n {request}");
            if (request.Content != null)
            {
                _output.WriteLine(await request.Content.ReadAsStringAsync());
            }
            _output.WriteLine("");

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            _output.WriteLine($"Response: \n {response} \n {await response.Content.ReadAsStringAsync()}");

            return response;
        }
    }
}