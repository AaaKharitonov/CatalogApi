using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CatalogApi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace CatalogApi.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CatalogsControllerAttribute : Attribute
    {
        public CatalogsControllerAttribute(string route) => Route = route;
        public string Route { get; set; }
    }

    public class CatalogsControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsGenericType)
            {
                var genericType = controller.ControllerType.GenericTypeArguments[0];
                var customNameAttribute = genericType.GetCustomAttribute<CatalogsControllerAttribute>();

                if (customNameAttribute?.Route != null)
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(customNameAttribute.Route)),
                    });
                }
                else
                {
                    controller.ControllerName = genericType.Name;
                }
            }
        }
    }

    public class CatalogsControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            Assembly currentAssembly = typeof(CatalogsControllerFeatureProvider).Assembly;
            IEnumerable<Type> types = currentAssembly.GetExportedTypes().Where(x => x.GetCustomAttributes<CatalogsControllerAttribute>().Any());

            types.ForEach(type => feature.Controllers.Add(typeof(CatalogsController<>).MakeGenericType(type).GetTypeInfo()));
        }
    }

    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    //exclude from swagger
    public class CatalogsController<T> : ControllerBase where T : Catalog
    {
        private readonly ICatalogsRepository<T> _rep;
        private readonly IQueries _queries;
        private readonly EntitiesInfoService _entitiesInfoService;

        public CatalogsController(ICatalogsRepository<T> rep, EntitiesInfoService entitiesInfoService, IQueries queries)
        {
            _rep = rep;
            _entitiesInfoService = entitiesInfoService;
            _queries = queries;
        }

        [HttpGet("dapper")]
        public async Task<ActionResult<IEnumerable<T>>> GetDapper() => (await _queries.GetAsync<T>()).ToList();

        [HttpGet("dapper/{id}")]
        public async Task<ActionResult<T>> GetDapper(int id)
        {
            var entity = await _queries.GetAsync<T>(id);

            if (entity == null) return NotFound();

            return entity;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> Get() => await _rep.GetAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> Get(int id)
        {
            var entity = await _rep.GetAsync(id);

            if (entity == null) return NotFound();

            return entity;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, T entity)
        {
            if (id != entity.Id) return BadRequest();

            try
            {
                await _rep.UpdateAsync(entity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<T>> Post(T entity)
        {
            await _rep.CreateAsync(entity);

            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<T>> Delete(int id)
        {
            var entity = await _rep.GetAsync(id);

            if (entity == null) return NotFound();

            await _rep.DeleteAsync(entity);

            return entity;
        }

        private async Task<bool> EntityExists(int id) => (await _rep.GetAsync()).Any(e => e.Id == id);
    }
}