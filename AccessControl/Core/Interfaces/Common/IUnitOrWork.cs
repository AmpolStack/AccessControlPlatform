
using AccessControl.Core.Models;

namespace AccessControl.Core.Interfaces.Common
{
    public interface IUnitOrWork
    {
        public Task BeginAsync();
        public Task CommitAsync();
        public Task RollbackAsync();

        AccessControlContext Context { get; }
    }
}
