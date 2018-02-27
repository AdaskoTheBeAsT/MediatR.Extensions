namespace MediatR.Extensions.log4net
{
    public static class MediatorBuilderExtensions
    {
        public static IMediatorBuilder UseLog4Net(this IMediatorBuilder builder)
        {
            builder.WithRequestDecorator("Log4Net", typeof (LoggingRequestHandler<,>));

            return builder;
        }
    }
}