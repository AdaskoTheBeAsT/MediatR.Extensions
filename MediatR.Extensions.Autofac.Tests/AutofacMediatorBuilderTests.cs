using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Autofac;

namespace MediatR.Extensions.Autofac.Tests
{
    public abstract class MediatorBuilderTests
    {
        protected abstract IMediatorBuilder GetMediatorBuilder();
        protected abstract Assembly GetTestAssembly();
        
        [Fact]
        public async Task Should_Register_Handler()
        {
            var mediator = GetMediatorBuilder()
                .WithRequestHandler(typeof (PingHandler))
                .Build();

            var pong = await mediator.Send(new Ping());
            
            Assert.Equal("Handled", pong.Message);
        }

        [Fact]
        public async Task Should_Register_All_Handlers_From_Assembly()
        {
            var mediator = GetMediatorBuilder()
                .WithRequestHandlerAssemblies(GetTestAssembly())
                .Build();

            var result1 = await mediator.Send(new Ping {Message = "One"});
            var result2 = await mediator.Send(new AnotherPing {Message = "Two"});

            Assert.Equal("OneHandled", result1.Message);
            Assert.Equal("TwoHandled", result2.Message);
        }

        [Fact]
        public async Task Should_Register_Decorator()
        {
            var mediator = GetMediatorBuilder()
                .WithRequestHandler(typeof(PingHandler))
                .WithRequestDecorator("decorator_one", typeof(DecoratorOne<,>))
                .WithRequestDecorator("decorator_two", typeof(DecoratorTwo<,>))
                .Build();

            var pong = await mediator.Send(new Ping());

            Assert.Equal("DecoratorTwoDecoratorOneHandled", pong.Message);
        }

        [Fact]
        public async Task Should_Register_NotificationHandler()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandler(typeof (NoteHandler))
                .Build();

            var notification = new Note();

            await mediator.Publish(notification);

            Assert.Equal(1, notification.Count);
        }

        [Fact]
        public async Task Should_Register_Multiple_NotificationHandlers()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandler(typeof (NoteHandler))
                .WithNotificationHandler(typeof (AnotherNoteHandler))
                .Build();

            var notification = new Note();

            await mediator.Publish(notification);

            Assert.Equal(2, notification.Count);
        }

        [Fact]
        public async Task Generic_Notification_Handler_Does_Handle_All_Inheriting_Notification_Types()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandler(typeof(GenericNotificationHandler))
                .Build();

            var notification = new Note();

            await mediator.Publish(notification);

            Assert.Equal(1, notification.Count);
        }

        [Fact]
        public async Task Should_Register_All_Notification_Handlers_In_Assembly()
        {
            var mediator = GetMediatorBuilder()
                .WithNotificationHandlerAssemblies(GetTestAssembly())
                .Build();

            var notification = new Note();

            await mediator.Publish(notification);

            Assert.Equal(3, notification.Count);
        }
    }

    public class AutofacMediatorBuilderTests : MediatorBuilderTests
    {
        private readonly ILifetimeScope container;

        public AutofacMediatorBuilderTests()
        {
            container = new ContainerBuilder().Build();
        }

        protected override IMediatorBuilder GetMediatorBuilder()
        {
            return new AutofacMediatorBuilder(new ContainerBuilder().Build());
        }

        protected override Assembly GetTestAssembly()
        {
            return typeof (AutofacMediatorBuilderTests).Assembly;
        }
    }
}