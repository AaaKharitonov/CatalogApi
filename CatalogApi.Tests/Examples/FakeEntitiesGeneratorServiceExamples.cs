using System;
using CatalogApi.DAL;
using CatalogApi.Domain.Blogs;
using CatalogApi.Domain.Catalogs;
using CatalogApi.Domain.Employees;
using CatalogApi.Tests.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace CatalogApi.Tests.Examples
{
    public class FakeEntitiesGeneratorServiceExamples
    {
        private readonly ITestOutputHelper _output;

        private readonly FakeEntitiesGeneratorService _generator;

        public FakeEntitiesGeneratorServiceExamples(ITestOutputHelper output)
        {
            this._output = output;

            Func<DefaultDbContext> contextFactory = () 
                => new DefaultDbContext(Utilities.Utilities.TestDbContextOptions());

            EntitiesInfoService entitiesInfoService = new EntitiesInfoService(contextFactory);

            _generator = new FakeEntitiesGeneratorService(entitiesInfoService);
        }

        [Theory]
        [InlineData(typeof(Department))]
        [InlineData(typeof(Employee))]
        [InlineData(typeof(SimpleCatalog))]
        [InlineData(typeof(User))]
        [InlineData(typeof(Blog))]
        [InlineData(typeof(Post))]
        [InlineData(typeof(Tag))]
        [InlineData(typeof(PostTag))]
        public void GenerateExample(Type type) => _output.WriteLine(_generator.Generate(type, 3));
    }
}