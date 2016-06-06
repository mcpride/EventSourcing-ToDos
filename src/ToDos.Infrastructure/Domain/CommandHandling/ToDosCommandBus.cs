using MS.EventSourcing.Infrastructure.MassTransit;

namespace ToDos.Infrastructure.Domain.CommandHandling
{
    public interface IToDosCommandBus : ICommandBus // marker interface for DI filtering
    {
    }

    public class ToDosCommandBus : CommandBus, IToDosCommandBus // marker class
    {
    }
}
