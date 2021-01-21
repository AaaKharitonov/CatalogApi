using System;

namespace CatalogApi.Domain
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
    }
}