using System;
using MassTransit;
using MS.EventSourcing.Infrastructure.CommandHandling;
using MS.Infrastructure;
using ToDos.Commands;
using ToDos.Domain.Aggregates;

namespace ToDos.Domain.CommandHandling
{
    public class ToDosCommandHandler : 
        Consumes<NotifyToDosContextInitialization>.Context,
        IToDosCommandHandler
    {
        private readonly IToDosContext _context;

        public ToDosCommandHandler(IToDosContext context)
        {
            _context = context;
        }

        public void Consume(IConsumeContext<NotifyToDosContextInitialization> consumeContext)
        {
            try
            {
                var aggregate = _context.GetById<ToDoList>(Uuid.Empty()) ?? new ToDoList(Uuid.Empty());
                aggregate.NotifyToDosContextInitialization();
                _context.Finalize(aggregate);
                consumeContext.Respond(CommandResult.Successful, context => context.SetExpirationTime(DateTime.Now.AddMinutes(1)));
            }
            catch (Exception ex)
            {
                consumeContext.Respond(new CommandResult(ex.Message), context => context.SetExpirationTime(DateTime.Now.AddMinutes(1)));
            }
        }
    }
}
