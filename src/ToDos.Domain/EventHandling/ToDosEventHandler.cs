using System;
using MassTransit;
using ToDos.Events;

namespace ToDos.Domain.EventHandling
{
    public class ToDosEventHandler : 
        Consumes<ToDosContextInitialized>.All, 
        IToDosEventHandler
    {
        public void Consume(ToDosContextInitialized message)
        {
            Console.WriteLine("Success: ToDos domain context has been initialized!");
        }
    }
}
