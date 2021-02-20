using ANQ.Messages.Config;
using ANQ.Messages.NonScheduledTasks.ContentService.Interfaces;
using ANQ.Messages.NonScheduledTasks.Rates;
using ANQ.Messages.NonScheduledTasks.UserProfile;
using ContentServiceManagementAPI.DapperImplementation;
using ContentServiceManagementAPI.Events;
using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI
{
    public static class EventsServiceCollections
    {
        public static IServiceCollection AddRabbitMqMassTransitServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<CreateUserRequestConsumer>();
            services.AddScoped<DNDServiceStatusConsumer>();
            services.AddScoped<DndClientUpdateConsumer>();
            services.AddScoped<DndSpUpdateConsumer>();
            services.AddScoped<UpdateTenantConsumer>();
            services.AddScoped<CompleteVasConfigurationConsumer>();
            services.AddTransient<IServiceRepository, ServiceRepository>();

            services.AddMassTransit(m =>
            {
                m.AddConsumer<CreateUserRequestConsumer>();
                m.AddConsumer<DNDServiceStatusConsumer>();
                m.AddConsumer<DndClientUpdateConsumer>();
                m.AddConsumer<DndSpUpdateConsumer>();
                m.AddConsumer<UpdateTenantConsumer>();
                m.AddConsumer<CompleteVasConfigurationConsumer>();
            });

            string RabbitMqHostIp = configuration["ScheduledTasksSettings:RabbitMqIp"];
            string RabbitMqUsername = configuration["ScheduledTasksSettings:RabbitMqUsername"];
            string RabbitMqPassword = configuration["ScheduledTasksSettings:RabbitMqPassword"];

            //publishing to consumer
            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(RabbitMqHostIp, "/", h =>
                {
                    h.Username(RabbitMqUsername);
                    h.Password(RabbitMqPassword);
                });

                //cfg.Host(RabbitMqHostIp, "notifications", h =>
                //{
                //    h.Username(RabbitMqUsername);
                //    h.Password(RabbitMqPassword);
                //});

                cfg.ReceiveEndpoint(RabbitMQEndpoints.CreateUserQueue, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(10, 100));
                    e.Consumer<CreateUserRequestConsumer>(provider);
                });

                cfg.ReceiveEndpoint(RabbitMQEndpoints.DNDServiceStatus, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(10, 100));
                    e.Consumer<DNDServiceStatusConsumer>(provider);
                });

                cfg.ReceiveEndpoint(RabbitMQEndpoints.UpdateClientDetailsOnContentManagement, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(10, 100));
                    e.Consumer<DndClientUpdateConsumer>(provider);
                });

                cfg.ReceiveEndpoint(RabbitMQEndpoints.UpdateSpDetailsOnContentManagement, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(10, 100));
                    e.Consumer<DndSpUpdateConsumer>(provider);
                });

                cfg.ReceiveEndpoint(RabbitMQEndpoints.UpdateTenants, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(10, 100));
                    e.Consumer<UpdateTenantConsumer>(provider);
                });

                cfg.ReceiveEndpoint(RabbitMQEndpoints.CompleteVhpVasConfiguration, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(10, 100));
                    e.Consumer<CompleteVasConfigurationConsumer>(provider);
                });

            }));

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<ICreateServiceRequest>());
            services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<IMapClientToServiceRequest>());
            services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<IUnMapClientToServiceRequest>());
            services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<IMapContentToServiceRequest>());
            services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<IServiceToClientRequest>());
            services.AddScoped(provider => provider.GetRequiredService<IBus>().CreateRequestClient<IContentToServiceProviderRequest>());


            services.AddSingleton<IHostedService, BusService>();

            return services;
        }
    }
}
