using Newtonsoft.Json;
using Xunit.Abstractions;

namespace CatalogApi.Tests.Utilities
{
    public static class TestOutputHelperExtension
    {
        public static void WriteLine(this ITestOutputHelper output, object obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            output.WriteLine(json);
        }
    }
}