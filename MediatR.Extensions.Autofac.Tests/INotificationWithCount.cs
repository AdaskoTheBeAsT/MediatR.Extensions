namespace MediatR.Extensions.Autofac.Tests
{
    public interface INotificationWithCount : INotification
    {
        int Count { get; set; }
    }
}