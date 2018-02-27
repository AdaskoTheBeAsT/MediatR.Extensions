using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace MediatR.Extensions.FluentValidation
{
    public class ValidationRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _innerHander;
        private readonly IValidator<TRequest>[] _validators;

        public ValidationRequestHandler(IRequestHandler<TRequest, TResponse> innerHandler, IValidator<TRequest>[] validators)
        {
            _validators = validators;
            _innerHander = innerHandler;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var context = new ValidationContext(request);

            var failures =
                _validators.Select(v => v.Validate(context)).SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return _innerHander.Handle(request, cancellationToken);
        }
    }
}