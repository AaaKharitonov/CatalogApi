using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.DAL
{
    public abstract class Catalog
    {
        public int Id { get; set; }
    }

    public interface ICatalogsRepository<TCatalog> where TCatalog : Catalog
    {
        Task<List<TCatalog>> GetAsync();
        Task<TCatalog> GetAsync(int id);
        Task CreateAsync(TCatalog item);
        Task UpdateAsync(TCatalog item);
        Task DeleteAsync(TCatalog item);
    }

    public class CatalogsRepository<TCatalog> : ICatalogsRepository<TCatalog> where TCatalog : Catalog
    {
        readonly DbContext _context;
        readonly DbSet<TCatalog> _dbSet;

        public CatalogsRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TCatalog>();
        }

        public async Task<List<TCatalog>> GetAsync() => await _dbSet.ToListAsync();
        public async Task<TCatalog> GetAsync(int id) => await _dbSet.FindAsync(id);

        public async Task CreateAsync(TCatalog item)
        {
            _dbSet.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TCatalog item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TCatalog item)
        {
            _dbSet.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}