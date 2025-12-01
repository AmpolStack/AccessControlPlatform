using AccessControl.Core.Interfaces.Repositories;
using AccessControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AccessControl.Infraestructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AccessControlContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AccessControlContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) =>
            await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) =>
            await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) =>
            await _dbSet.AddRangeAsync(entities);

        public void Update(T entity) =>
            _dbSet.Update(entity);

        public void Remove(T entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) =>
            _dbSet.RemoveRange(entities);

        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
