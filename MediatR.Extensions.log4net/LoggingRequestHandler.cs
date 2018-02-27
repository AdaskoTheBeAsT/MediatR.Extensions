using log4net;

namespace MediatR.Extensions.log4net
{
    using System.Threading;
    using System.Threading.Tasks;

    public class LoggingRequestHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHander;
        private readonly ILog _log;

        public LoggingRequestHandler(IRequestHandler<TRequest, TResponse> innerHandler)
        {
            _innerHander = innerHandler;
            _log = LogManager.GetLogger(innerHandler.GetType());
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            _log.Info(string.Format("Request: {0}", request));
            var response = await _innerHander.Handle(request, cancellationToken);
            _log.Info(string.Format("Response: {0}", response));

            return response;
        }
    }
}