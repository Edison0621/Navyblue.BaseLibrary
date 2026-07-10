using System.Collections.Generic;

namespace Navyblue.Foundation.Cqrs
{
    public interface IOutboxDrain
    {
        IEnumerable<Event> Drain();
    }
}