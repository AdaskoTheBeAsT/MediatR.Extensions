using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Features.Variance;

namespace MediatR.Extensions.Autofac
{
    using System.Collections.Generic;
    using CommonServiceLocator;

    public class AutofacMediatorBuilder : MediatorBuilder
    {
        private const string HandlerKey = "handler";
        private readonly ILifetimeScope _container;
        private readonly ContainerBuilder _builder;
        private string _key;

        public AutofacMediatorBuilder(ILifetimeScope container)
        {
            _key = HandlerKey;
            _container = container;
            _builder = new ContainerBuilder();
        }

        protected override void RegisterRequestDecorator(string name, Type decoratorType)
        {
            _builder.RegisterGenericDecorator(decoratorType, typeof(IRequestHandler<,>),
                    fromKey: _key).Named(name, typeof(IRequestHandler<,>));

            _key = name;
        }

        protected override void RegisterRequestHandlersFromAssembly(Assembly assembly)
        {
            _builder.RegisterAssemblyTypes(assembly).As(t => t.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(IRequestHandler<,>)))
                .Select(i => new KeyedService(HandlerKey, i)));
        }

        protected override void RegisterRequestHandler(Type type)
        {
            var services = type.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof (IRequestHandler<,>)))
                .Select(i => new KeyedService(HandlerKey, i) as Service);

            _builder.RegisterType(type).As(services.ToArray());
        }

        protected override void RegisterNotificationHandler(Type notificationHandlerType)
        {
            _builder.RegisterType(notificationHandlerType)
                .As(notificationHandlerType.GetInterfaces()
                    .Where(i => i.IsClosedTypeOf(typeof(INotificationHandler<>))).ToArray());
        }

        protected override void RegisterNotificationHandlersFromAssembly(Assembly assembly)
        {
            _builder.RegisterAssemblyTypes(assembly)
                .As(t => t.GetInterfaces().Where(i => i.IsClosedTypeOf(typeof (INotificationHandler<>))).ToArray());
        }
      
        protected override IMediator BuildMediator()
        {
            _builder.RegisterSource(new ContravariantRegistrationSource());
            _builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
            _builder.Register<SingleInstanceFactory>(
                context =>
                {
                    var componentContext = context.Resolve<IComponentContext>();
                    return type => componentContext.Resolve(type);
                });
            _builder.Register<MultiInstanceFactory>(
                context =>
                {
                    var componentContext = context.Resolve<IComponentContext>();
                    return type => (IEnumerable<object>)componentContext.Resolve(
                        typeof(IEnumerable<>).MakeGenericType(type));
                });

            var lazy = new Lazy<IServiceLocator>(() => new AutofacServiceLocator(_container));
            var serviceLocatorProvider = new ServiceLocatorProvider(() => lazy.Value);

            _builder.RegisterInstance(serviceLocatorProvider);

            _builder.RegisterGenericDecorator(typeof(WrapperRequestHandler<,>), typeof(IRequestHandler<,>),
                fromKey: _key);
#pragma warning disable 612, 618
            _builder.Update(_container.ComponentRegistry);
#pragma warning restore 612, 618
            return serviceLocatorProvider().GetInstance<IMediator>();
        }
    }
}