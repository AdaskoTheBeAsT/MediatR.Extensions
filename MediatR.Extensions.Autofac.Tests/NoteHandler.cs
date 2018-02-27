using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    using System.Threading;

    public class NoteHandler
        : INotificationHandler<Note>
    {
        public Task Handle(Note notification, CancellationToken cancellationToken)
        {
            notification.Count++;

#if NET452
            return Task.Delay(0);
#else   
            return Task.CompletedTask;
#endif
        }
    }


    public class GenericNotificationHandler
        : INotificationHandler<INotificationWithCount>
    {
        public Task Handle(INotificationWithCount notification, CancellationToken cancellationToken)
        {
            notification.Count++;
            
#if NET452
            return Task.Delay(0);
#else   
            return Task.CompletedTask;
#endif
        }
    }

    public class AnotherNoteHandler
        : INotificationHandler<Note>
    {
        public Task Handle(Note notification, CancellationToken cancellationToken)
        {
            notification.Count++;
            
#if NET452
            return Task.Delay(0);
#else   
            return Task.CompletedTask;
#endif
        }
    }
    
    public class Note : INotificationWithCount
    {
        public int Count { get; set; }
    }
}