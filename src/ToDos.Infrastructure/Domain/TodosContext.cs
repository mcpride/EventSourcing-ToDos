using System;
using System.Linq;
using MassTransit.SubscriptionConfigurators;
using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.EventHandling;
using ToDos.Commands;
using ToDos.Domain;
using ToDos.Infrastructure.Domain.CommandHandling;
using ToDos.Infrastructure.EventHandling;

namespace ToDos.Infrastructure.Domain
{
    public class ToDosContext : DomainContext, IToDosContext
    {
        private readonly Lazy<IToDosCommandBus> _commandBus;
        private IToDosEventBus _eventBus;
        private bool _initialized;

        public ToDosContext(Lazy<IToDosCommandBus> commandBus, IToDosEventBus eventBus, IEventStore eventStore, ISnapshotStore snapshotStore, IDomainRepository domainRepository) 
            : base(eventBus, eventStore, snapshotStore, domainRepository)
        {
            _commandBus = commandBus;
            _eventBus = eventBus;
        }

        //Just for in-memory test initialization:
        public void InitializeLoopback(Action<SubscriptionBusServiceConfigurator> subscribeAllHandlers)
        {
            if (_initialized) return;
            _eventBus.Initialize(subscribeAllHandlers, sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/todos_events")));
            _commandBus.Value.InitializeWithServiceBus(_eventBus.ServiceBus);
            _commandBus.Value.Send(new NotifyToDosContextInitialization(), result =>
            {
                if (!result.Success && result.Errors.Any())
                {
                    Console.Error.WriteLine(result.Errors.First());
                }
            }, TimeSpan.FromSeconds(30));
            _initialized = true;
        }

        public void Initialize(Action<SubscriptionBusServiceConfigurator> subscribeCommandHandlers, Action<SubscriptionBusServiceConfigurator> subscribeEventHandlers)
        {
            if (_initialized) return;

            throw new NotImplementedException();

            //_initialized = true;
        }


        #region Disposable pattern

        private bool _disposed;

        ~ToDosContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                _eventBus.Dispose();
            }

            // release any unmanaged objects
            // set thick object references to null
            _eventBus = null;
            _disposed = true;
        }

        #endregion Disposable pattern

    }
}
