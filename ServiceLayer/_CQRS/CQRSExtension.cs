using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using NetCore.AutoRegisterDi;
using System;
using ServiceLayer._CQRS.SubscriptionCommands;

namespace ServiceLayer._CQRS
{
    public static class CQRSExtension
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services)
        {
            services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(DisableSubCommandHandler)))
                .Where(c => c.Name.EndsWith("Handler"))
                .AsPublicImplementedInterfaces();

            RegisterQueryServices(services);
            RegisterCommandServices(services);

            return services;
        }

        private static void RegisterQueryServices(IServiceCollection services)
        {
            services.AddScoped(provider => new Func<Type, IHandleQuery>(
                    (type) => (IHandleQuery)provider.GetRequiredService(type)
                ));

            services.AddScoped<IQueriesBus, QueriesBus>();
        }

        private static void RegisterCommandServices(IServiceCollection services)
        {
            services.AddScoped(provider => new Func<Type, IHandleCommand>(
                    (type) => (IHandleCommand)provider.GetRequiredService(type)
                ));

            services.AddScoped<ICommandsBus, CommandsBus>();
        }
    }
}
