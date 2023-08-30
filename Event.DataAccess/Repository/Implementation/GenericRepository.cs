using Event.DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Event.Data.Context;

namespace Event.DataAccess.Repository.Implementation
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EventDbContext _dbContext;
        private DbSet<T> table;

        public GenericRepository(EventDbContext dbContext)
        {
            _dbContext = dbContext;
            table = _dbContext.Set<T>();
        }

        public async Task Add(T entity)
        {
            //await CreateSavePointAsync("New Afiari Save Point");
            await table.AddAsync(entity);
        }

        public async Task AddRange(IList<T> entity)
        {
            //await CreateSavePointAsync("New Afiari Save Point");
            await table.AddRangeAsync(entity);
        }

        public async Task Delete(Guid EntityId)
        {
            var entity = await ReadSingle(EntityId);
            table.Remove(entity);
        }

        public void DeleteRange(IList<T> entity)
        {
            table.RemoveRange(entity);
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await table.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> ReadAll()
        {
            return await table.ToListAsync();
        }

        public IQueryable<T> ReadAllQuery()
        {
            return table.AsQueryable();
        }

        public async Task<T> ReadSingle(Guid EntityId)
        {
            return await table.FindAsync(EntityId);
        }

        public void Update(T Entity)
        {
            //table.Update(Entity);
            table.Attach(Entity);
            _dbContext.Entry(Entity).State = EntityState.Modified;
        }

        public void UpdateRange(IList<T> entities)
        {
            table.UpdateRange(entities);
        }
    }
}
