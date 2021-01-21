using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CatalogApi.Controllers;
using CatalogApi.DAL;
using CatalogApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CatalogApi
{
    public class EntityInfo
    {
        public EntityInfo(Type type, string tableName, string route)
        {
            Type = type;
            TableName = tableName;
            Route = route;
        }

        public string TableName { get; }
        public Type Type { get; }
        public string Route { get; }

        public override string ToString() =>
            new StringBuilder()
                .AppendLine($"TypeName: {Type.Name}")
                .AppendLine($"TableName: {TableName}")
                .AppendLine($"Route: {Route}")
                .ToString();
    }

    public class EntitiesInfoService
    {
        private readonly Func<DefaultDbContext> _contextFactory;
        private Dictionary<Type, EntityInfo> _dict;

        public EntitiesInfoService(Func<DefaultDbContext> contextFactory) => _contextFactory = contextFactory;

        public void Init()
        {
            IEnumerable<IEntityType> entityTypes = null;

            using (var context = _contextFactory())
                entityTypes = context.Model.GetEntityTypes();

            _dict = entityTypes.Select(entityType =>
            {
                Type type = entityType.ClrType;
                string tableName = entityType.GetTableName();

                string route = GetRoute(type);

                return new EntityInfo(type, tableName, route);
            }).ToDictionary(x => x.Type);
        }

        private string GetRoute(Type type)
        {
            IEnumerable<CatalogsControllerAttribute> attrs = type.GetCustomAttributes<CatalogsControllerAttribute>();

            if (attrs.Any())
            {
                CatalogsControllerAttribute attr = (CatalogsControllerAttribute)type
                    .GetCustomAttributes(typeof(CatalogsControllerAttribute), true).FirstOrDefault();

                if (attr != null) return attr.Route;
            }

            return null;
        }

        //Warning!!! no checks
        public EntityInfo this[Type type] => _dict[type];
        public EntityInfo this[string route] => _dict.SingleOrDefault(x => x.Value.Route == route).Value;

        public IEnumerable<string> GetTableNames() => _dict.Select(x => x.Value.TableName);
        public IEnumerable<string> GetRoutes() => _dict.Select(x => x.Value.Route);
        public IEnumerable<Type> GetTypes() => _dict.Select(x => x.Key);

        public IEnumerable<EntityInfo> GetInfos() => _dict.Values;

        public string GetInfo(Type type) => _dict[type].ToString();
    }
}