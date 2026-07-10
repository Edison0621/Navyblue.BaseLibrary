using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public interface IOutbox
    {
        Task SaveAsync(Event @event);
        Task SaveAsync(IEnumerable<Event> events);
    }
}