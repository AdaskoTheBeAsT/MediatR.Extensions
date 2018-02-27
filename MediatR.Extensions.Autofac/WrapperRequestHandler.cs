using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac
{
    using System.Threading;

    internal class WrapperRequestHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHandler;

        public WrapperRequestHandler(IRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHandler = innerHandler;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return _innerHandler.Handle(request, cancellationToken);
        }
    }
}