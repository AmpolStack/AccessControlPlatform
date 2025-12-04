using AccessControl.Domain.Models;

namespace AccessControl.Domain.Services
{
    public interface IUnitOrWork
    {
        public Task BeginAsync();
        public Task CommitAsync();
        public Task RollbackAsync();

        AccessControlContext Context { get; }
    }
}
