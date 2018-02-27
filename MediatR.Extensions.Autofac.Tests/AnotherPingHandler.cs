using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    using System.Threading;

    public class AnotherPingHandler
        : IRequestHandler<AnotherPing, AnotherPong>
    {
        public Task<AnotherPong> Handle(AnotherPing request, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new AnotherPong
                {
                    Message = string.Format("{0}Handled", request.Message)
                });
        }
    }

    public class AnotherPing : IRequestWithMessage<AnotherPong>
    {
        public string Message { get; set; }
    }

    public class AnotherPong
    {
        public string Message { get; set; }
    }
}