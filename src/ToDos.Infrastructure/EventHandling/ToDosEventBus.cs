using MS.EventSourcing.Infrastructure.MassTransit;

namespace ToDos.Infrastructure.EventHandling
{
    public interface IToDosEventBus : IEventBus // marker interface for DI filtering
    {
    }

    public class ToDosEventBus: EventBus, IToDosEventBus // marker class
    {
    }
}
