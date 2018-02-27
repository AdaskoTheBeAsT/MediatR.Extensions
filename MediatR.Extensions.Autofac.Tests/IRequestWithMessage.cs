namespace MediatR.Extensions.Autofac.Tests
{
    public interface IRequestWithMessage<out T>
        : IRequest<T>
    {
        string Message { get; set; }
    }
}