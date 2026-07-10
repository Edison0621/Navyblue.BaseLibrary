using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public interface IInboxStore
    {
        Task<bool> TryGetResponse<TResponse>(string requestId, out TResponse response);
        Task SetResponse<TResponse>(string requestId, TResponse response);
        Task<bool> IsProcessedAsync(string requestId);
        Task MarkProcessedAsync(string requestId);
    }
}