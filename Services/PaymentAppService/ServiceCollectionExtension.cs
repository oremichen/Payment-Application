using Microsoft.Extensions.DependencyInjection;
using PaymentAppService.PaymentService;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentAppService
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddPaymentApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IPaymentApplicationService, PaymentApplicationService>();
            return services;
        }
    }
}
