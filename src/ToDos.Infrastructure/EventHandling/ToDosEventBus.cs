using MassTransit;
using MS.EventSourcing.Infrastructure.MassTransit;

namespace ToDos.Infrastructure.EventHandling
{
    public interface IToDosEventBus : IEventBus // marker interface for DI filtering
    {
        IServiceBus ServiceBus { get; }
    }

    public class ToDosEventBus: EventBus, IToDosEventBus // marker class
    {
        IServiceBus IToDosEventBus.ServiceBus
        {
            get { return ServiceBus; }
        }
    }
}
