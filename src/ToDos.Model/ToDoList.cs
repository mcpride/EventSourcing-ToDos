using System.Collections.Generic;

namespace ToDos.Models
{
    public class ToDoList
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ToDoItem> Items { get; private set; }
        public ToDoList()
        {
            Items = new List<ToDoItem>();
        }
    }
}
