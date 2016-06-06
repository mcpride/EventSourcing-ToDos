using MS.EventSourcing.Infrastructure.Domain;
using MS.Infrastructure;

namespace ToDos.Domain.Entities
{
    internal class ToDoItem : Entity
    {
        public ToDoItem(AggregateRoot parent, Uuid entityId) : base(parent, entityId)
        {
        }
    }
}
