using System;
using MassTransit.SubscriptionConfigurators;
using MS.EventSourcing.Infrastructure.Domain;
using MS.EventSourcing.Infrastructure.EventHandling;
using ToDos.Events;
using ToDos.Infrastructure.EventHandling;

namespace ToDos.Infrastructure.Domain
{
    public class ToDosContext : DomainContext, IToDosContext
    {
        private IToDosEventBus _eventBus;

        public ToDosContext(IToDosEventBus eventBus, IEventStore eventStore, ISnapshotStore snapshotStore, IDomainRepository domainRepository) 
            : base(eventBus, eventStore, snapshotStore, domainRepository)
        {
            _eventBus = eventBus;
        }

        public void Initialize(Action<SubscriptionBusServiceConfigurator> subscribeEventHandlers)
        {
            _eventBus.Initialize(subscribeEventHandlers, sbc => sbc.ReceiveFrom(new Uri("loopback://localhost/todos_events")));
            _eventBus.PublishEvent(new ToDosContextInitialized());
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
