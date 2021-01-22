using System;
using System.Linq;
using System.Threading.Tasks;
using CatalogApi.DAL;
using CatalogApi.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace CatalogApi.Tests.Examples
{
    public class DefaultDbContextTest
    {
        private readonly ITestOutputHelper _output;
        private readonly FakeEntitiesGeneratorService _generator;

        public DefaultDbContextTest(ITestOutputHelper output)
        {
            this._output = output;

            Func<DefaultDbContext> contextFactory = ()
                => new DefaultDbContext(Utilities.Utilities.TestDbContextOptions());

            EntitiesInfoService entitiesInfoService = new EntitiesInfoService(contextFactory);

            _generator = new FakeEntitiesGeneratorService(entitiesInfoService);
        }

        [Fact]
        public async Task GetDepartmentsAsync_DepartmentsAreReturned()
        {
            using (var db = new DefaultDbContext(Utilities.Utilities.TestDbContextOptions()))
            {
                // Arrange

                var expected = _generator.Generate<Department>(5);
                await db.Departments.AddRangeAsync(expected);
                await db.SaveChangesAsync();

                // Act
                var actual = await db.Departments.ToListAsync();

                // Assert
                Assert.Equal(
                    expected.OrderBy(m => m.Id).Select(m => m.Name),
                    actual.OrderBy(m => m.Id).Select(m => m.Name));
            }
        }

        [Fact]
        public async Task AddDepartmentsAsync_DepartmentsIsAdded()
        {
            using (var db = new DefaultDbContext(Utilities.Utilities.TestDbContextOptions()))
            {
                // Arrange
                var recId = Guid.NewGuid();
                var expected = new Department() { Id = recId, Name = "Name"};

                // Act
                await db.Departments.AddAsync(expected);

                // Assert
                var actual = await db.FindAsync<Department>(recId);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async Task DeleteAllDepartmentsAsync_DepartmentsAreDeleted()
        {
            using (var db = new DefaultDbContext(Utilities.Utilities.TestDbContextOptions()))
            {
                // Arrange
                var expected = _generator.Generate<Department>(5);
                await db.AddRangeAsync(expected);
                await db.SaveChangesAsync();

                // Act
                db.Departments.RemoveRange(expected);
                await db.SaveChangesAsync();

                // Assert
                Assert.Empty(await db.Departments.AsNoTracking().ToListAsync());
            }
        }

        [Fact]
        public async Task DeleteDepartmentsAsync_NoDepartmentIsDeleted_WhenDepartmentIsNotFound()
        {
            using (var db = new DefaultDbContext(Utilities.Utilities.TestDbContextOptions()))
            {
                // Arrange
                var expected = _generator.Generate<Department>(5);
                await db.AddRangeAsync(expected);
                await db.SaveChangesAsync();
                var recId = Guid.NewGuid();

                // Act
                try
                {
                    var department = await db.Departments.FindAsync(recId);
                    db.Departments.Remove(department);
                    await db.SaveChangesAsync();
                }
                catch
                {
                    // recId doesn't exist
                }

                // Assert
                var actual = await db.Departments.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expected.OrderBy(m => m.Id).Select(m => m.Name),
                    actual.OrderBy(m => m.Id).Select(m => m.Name));
            }
        }
    }
}