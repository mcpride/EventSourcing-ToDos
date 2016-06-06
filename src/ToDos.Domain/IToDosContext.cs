using System;
using MassTransit.SubscriptionConfigurators;
using MS.EventSourcing.Infrastructure.Domain;

namespace ToDos.Domain
{
    public interface IToDosContext: IDomainContext, IDisposable
    {
        void InitializeLoopback(Action<SubscriptionBusServiceConfigurator> subscribeAllHandlers);
        void Initialize(Action<SubscriptionBusServiceConfigurator> subscribeCommandHandlers, Action<SubscriptionBusServiceConfigurator> subscribeEventHandlers);
    }
}