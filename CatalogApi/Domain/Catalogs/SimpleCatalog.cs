using CatalogApi.Controllers;
using CatalogApi.DAL;

namespace CatalogApi.Domain.Catalogs
{
    [CatalogsController("simple")]
    public class SimpleCatalog : Catalog
    {
        public string Name { get; set; }
        public int Val { get; set; }
    }
}
