using AccessControl.Domain.Models;
using AccessControl.Domain.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccessControl.Infraestructure.Services
{
    public class UnitOfWork : IUnitOrWork
    {
        private readonly AccessControlContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AccessControlContext context)
        {
            _context = context;
        }

        public AccessControlContext Context => _context;

        public async Task BeginAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public Task CommitAsync() => _transaction!.CommitAsync();
        public Task RollbackAsync() => _transaction!.RollbackAsync();
    }
}
