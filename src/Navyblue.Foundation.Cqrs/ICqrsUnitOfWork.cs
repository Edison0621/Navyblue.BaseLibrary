using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public interface ICqrsUnitOfWork
    {
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}