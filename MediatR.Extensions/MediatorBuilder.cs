using System;
using System.Linq;
using System.Reflection;

namespace MediatR.Extensions
{
    public abstract class MediatorBuilder : IMediatorBuilder
    {
        private bool _isBuilt;
        private static Func<Type, bool> CreateGenericTypePredicate(Type type)
        {

            return i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == type;
            };
        }

        public IMediatorBuilder WithRequestDecorator(string name, Type decoratorType)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call AddRequestDecorator after Build() has been called");
            }

            var decoratorTypeInfo = decoratorType.GetTypeInfo();

#if NETSTANDARD1_1 || NETSTANDARD1_3
            var interfaces = decoratorTypeInfo.ImplementedInterfaces;
#else
            var interfaces = decoratorTypeInfo.GetInterfaces();
#endif

            if (interfaces.Any(CreateGenericTypePredicate(typeof(IRequestHandler<,>))))
            {
                RegisterRequestDecorator(name, decoratorType);
            }
            else
            {
                throw new ArgumentException(
                    "Decorator type must implement IRequestHandler<TRequest,TResponse> or IAsyncRequestHandler<TRequest, TResponse>",
                    nameof(decoratorType));
            }

            return this;
        }

        public IMediatorBuilder WithRequestHandler(Type requestHandlerType)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call WithRequestHandler after Build() has been called");
            }

            var requestHandlerTypeInfo = requestHandlerType.GetTypeInfo();

#if NETSTANDARD1_1 || NETSTANDARD1_3
            var interfaces = requestHandlerTypeInfo.ImplementedInterfaces;
#else
            var interfaces = requestHandlerTypeInfo.GetInterfaces();
#endif

            if (interfaces.Any(CreateGenericTypePredicate(typeof(IRequestHandler<,>))))
            {
                RegisterRequestHandler(requestHandlerType);
            }
            else
            {
                throw new ArgumentException(
                    "Handler type must implement IRe IRequestHandler<TRequest,TResponse> or IAsyncRequestHandler<TRequest, TResponse>",
                    nameof(requestHandlerType));
            }

            return this;
        }

        public IMediatorBuilder WithRequestHandlerAssemblies(params Assembly[] assemblies)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call AddRequestDecorator after Build() has been called");
            }

            foreach (var assembly in assemblies)
            {
                RegisterRequestHandlersFromAssembly(assembly);
            }

            return this;
        }

        public IMediatorBuilder WithNotificationHandler(Type notificationHandlerType)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call WithNotificationHandler after Build() has been called");
            }

            var notificationHandlerTypeInfo = notificationHandlerType.GetTypeInfo();

#if NETSTANDARD1_1 || NETSTANDARD1_3
            var interfaces = notificationHandlerTypeInfo.ImplementedInterfaces;
#else
            var interfaces = notificationHandlerTypeInfo.GetInterfaces();
#endif

            if (interfaces.Any(CreateGenericTypePredicate(typeof(INotificationHandler<>))))
            {
                RegisterNotificationHandler(notificationHandlerType);
            }
            else
            {
                throw new ArgumentException(
                    "Handler type must implement IRequestHandler<TRequest,TResponse> or IAsyncRequestHandler<TRequest, TResponse>",
                    nameof(notificationHandlerType));
            }

            return this;
        }

        public IMediatorBuilder WithNotificationHandlerAssemblies(params Assembly[] assemblies)
        {
            if (_isBuilt)
            {
                throw new Exception("Cannot call WithNotificationHandlerAssemblies after Build() has been called");
            }

            foreach (var assembly in assemblies)
            {
                RegisterNotificationHandlersFromAssembly(assembly);
            }

            return this;
        }

        public IMediator Build()
        {
            if (_isBuilt)
            {
                throw new Exception("Build() can only be called once");
            }

            var mediator = BuildMediator();

            _isBuilt = true;

            return mediator;
        }

        protected abstract void RegisterRequestHandler(Type handlerType);
        protected abstract void RegisterRequestDecorator(string name, Type decoratorType);
        protected abstract void RegisterRequestHandlersFromAssembly(Assembly assembly);


        protected abstract void RegisterNotificationHandler(Type notificationHandlerType);
        protected abstract void RegisterNotificationHandlersFromAssembly(Assembly assembly);


        protected abstract IMediator BuildMediator();
    }
}