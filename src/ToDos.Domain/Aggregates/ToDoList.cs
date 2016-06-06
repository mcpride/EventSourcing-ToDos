using MS.EventSourcing.Infrastructure.Domain;
using ToDos.Events;

namespace ToDos.Domain.Aggregates
{
    internal partial class ToDoList : AggregateRoot
    {
        public void NotifyToDosContextInitialization()
        {
            // Very complex logic here ;-)
            ApplyEvent(new ToDosContextInitialized());
        }
    }
}
