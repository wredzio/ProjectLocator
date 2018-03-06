using Autofac;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ProjectLocator.Shared.MediatR
{
    public static class MediatRConfig
    {
        public static void ConfigureMediatR(this ContainerBuilder containerBuilder)
        {
            var mediatorOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(IRequestHandler<>),
                typeof(AsyncRequestHandler<,>),
                typeof(AsyncRequestHandler<>)
            };

            foreach(var mediatorOpenType in mediatorOpenTypes)
            {
                containerBuilder
                    .RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatorOpenType)
                    .AsImplementedInterfaces();
            }
        }
    }
}
