using Event.DataAccess.Repository.Implementation;
using Event.DataAccess.Repository.Interface;
using Event.DataAccess.UnitOfWork.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Event.Data.Context;

namespace Event.DataAccess.UnitOfWork.Implementation
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly EventDbContext _context;
        private IExecutionStrategy strategy;
        private IGenericRepository<T> _repository;
        private IDbContextTransaction Transaction;

        private readonly string savepoint = "dbcontext save point";

        public UnitOfWork(EventDbContext dbContext)
        {
            _context = dbContext;
        }

        public IGenericRepository<T> Repository => _repository ??= new GenericRepository<T>(_context);

        private async Task RollBack()
        {
            await Transaction.RollbackAsync();
        }

        public async Task<bool> SaveAsync()
        {
            bool result = false;
            try
            {
                strategy = _context.Database.CreateExecutionStrategy();

                await strategy.Execute(async () =>
                {
                    Transaction = await _context.Database.BeginTransactionAsync();
                    await Transaction.CreateSavepointAsync(savepoint);
                    result = await _context.SaveChangesAsync() >= 0;

                    await Transaction.CommitAsync();
                });
            }
            catch (Exception e)
            {
                await RollBack();
                throw new Exception(e.Message.Equals("An error occurred while updating the entries. See the inner exception for details.") ? e.InnerException.Message : e.Message);
            }
            finally
            {
                await Transaction.DisposeAsync();
            }

            return result;
        }
    }
}
