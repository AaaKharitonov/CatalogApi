using System;
using Xunit;
using Xunit.Abstractions;

namespace CatalogApi.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public void Test1()
        {
            _output.WriteLine("Test1");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void Test2(int val)
        {
            _output.WriteLine($"Test2 val: {val}");
        }

    }
}
