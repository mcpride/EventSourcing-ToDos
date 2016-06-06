using MS.Infrastructure;

namespace ToDos.Domain.Aggregates
{
    partial class ToDoList
    {
        // ReSharper disable once RedundantBaseConstructorCall
        public ToDoList()
            : base()
        {
        }

        public ToDoList(Uuid id)
            : base(id)
        {
        }

    }
}
