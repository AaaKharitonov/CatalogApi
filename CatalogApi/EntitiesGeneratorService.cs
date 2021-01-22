using System;
using System.Collections.Generic;
using Bogus;
using CatalogApi.Domain.Blogs;
using CatalogApi.Domain.Catalogs;
using CatalogApi.Domain.Employees;

namespace CatalogApi
{
    public class FakeEntitiesGeneratorService
    {
        private readonly EntitiesInfoService _infoService;
        private readonly Dictionary<Type, Func<int, IEnumerable<object>>> _dict;

        public FakeEntitiesGeneratorService(EntitiesInfoService infoService)
        {
            _infoService = infoService;
            _dict = new Dictionary<Type, Func<int, IEnumerable<object>>>();
            SetGenerateFuncs();
        }

        private void SetGenerateFuncs()
        {
            _dict[typeof(Department)] = len =>
            {
                var fake = new Faker<Department>()
                    .StrictMode(true)
                    .RuleFor(x => x.Id, x => Guid.NewGuid())
                    .RuleFor(x => x.Name, x => x.Commerce.Department());

                return fake.Generate(len);
            };

            _dict[typeof(Employee)] = len =>
            {
                var fake = new Faker<Employee>()
                        .StrictMode(true)
                        .RuleFor(x => x.Id, x => Guid.NewGuid())
                        .RuleFor(x => x.Name, x => x.Name.FirstName())
                        .RuleFor(x => x.Birthday,
                            x => x.Date.Between(DateTime.Today.AddYears(-100), DateTime.Today))
                        .RuleFor(x => x.Salary, x => x.Random.Decimal(1000, 20000))

                        //todo
                        //.RuleFor(x => x.ManagerId, x => x.PickRandom(managerIds))

                        .RuleFor(x => x.Department, x => null)
                    ;

                return fake.Generate(len);
            };

            _dict[typeof(SimpleCatalog)] = len =>
            {
                var i = 0;

                var fake = new Faker<SimpleCatalog>()
                        .StrictMode(true)
                        .RuleFor(x => x.Id, x => i++)
                        .RuleFor(x => x.Name, x => x.Random.Word())
                        .RuleFor(x => x.Val, x => x.Random.Int(0, 1000000))
                    ;

                return fake.Generate(len);
            };

            #region Blogs

            _dict[typeof(User)] = len =>
            {
                var fake = new Faker<User>()
                        //.StrictMode(true)
                        .RuleFor(x => x.Id, x => Guid.NewGuid())
                        .RuleFor(x => x.UserName, x => x.Name.FirstName())
                    ;

                return fake.Generate(len);
            };

            _dict[typeof(Blog)] = len =>
            {
                var fake = new Faker<Blog>()
                        //.StrictMode(true)
                        .RuleFor(x => x.Id, x => Guid.NewGuid())
                        .RuleFor(x => x.Url, x => x.Internet.Url())
                        .RuleFor(x => x.UserId, x => x.Random.Int(1, 10))
                    ;

                return fake.Generate(len);
            };

            _dict[typeof(Post)] = len =>
            {
                var fake = new Faker<Post>()
                        //.StrictMode(true)
                        .RuleFor(x => x.Id, x => Guid.NewGuid())
                        .RuleFor(x => x.Title, x => x.Random.Words(3))
                        .RuleFor(x => x.Content, x => x.Lorem.Sentences())

                        //todo
                        .RuleFor(x => x.BlogId, x => Guid.NewGuid())
                    ;

                return fake.Generate(len);
            };

            _dict[typeof(Tag)] = len =>
            {
                var fake = new Faker<Tag>()
                        //.StrictMode(true)
                        .RuleFor(x => x.Id, x => Guid.NewGuid())
                        .RuleFor(x => x.Name, x => x.Random.Word())
                    ;

                return fake.Generate(len);
            };

            _dict[typeof(PostTag)] = len =>
            {
                var fake = new Faker<PostTag>()
                    //.StrictMode(true)
                    //todo
                    //.RuleFor(x => x.TagId, x => x.Random.Int(1, 10))
                    //.RuleFor(x => x.PostId, x => x.Random.Int(1, 10))
                    ;

                return fake.Generate(len);
            };

            #endregion


        }

        public IEnumerable<T> Generate<T>(int len)
        {
            if (_dict.TryGetValue(typeof(T), out Func<int, IEnumerable<object>> func) )
            {
                return (IEnumerable<T>)_dict[typeof(T)](len);
            }

            return null;
        }

        public IEnumerable<object> Generate(Type type, int len)
        {
            if (_dict.TryGetValue(type, out Func<int, IEnumerable<object>> func) )
            {
                return _dict[type](len);
            }

            return null;
        }

        public IEnumerable<object> Generate(string route, int len)
        {
            var info = _infoService[route];

            if (info == null)
                return null;
            
            return Generate(info.Type, len);
        }
    }
}