using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    using System.Threading;

    public class PingHandler
        : IRequestHandler<Ping, Pong>
    {
        public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new Pong
                {
                    Message = string.Format("{0}Handled", request.Message)
                });
        }
    }

    public class Ping
        : IRequestWithMessage<Pong>
    {
        public string Message { get; set; }
    }

    public class Pong
    {
        public string Message { get; set; }
    }
}