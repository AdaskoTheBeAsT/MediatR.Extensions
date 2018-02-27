using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    using System.Threading;

    public class DecoratorOne<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequestWithMessage<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHander;

        public DecoratorOne(IRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHander = innerHandler;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            request.Message += "DecoratorOne";
            return _innerHander.Handle(request, cancellationToken);
        }
    }
}