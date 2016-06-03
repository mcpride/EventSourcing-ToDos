using System;
using MassTransit.SubscriptionConfigurators;

namespace ToDos.Infrastructure.Domain
{
    public interface IToDosContext: IDisposable
    {
        void Initialize(Action<SubscriptionBusServiceConfigurator> subscribeEventHandlers);
    }
}