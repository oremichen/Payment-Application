using Microsoft.Extensions.DependencyInjection;
using PaymentRepository.PaymentGateway.CheapGateway;
using PaymentRepository.PaymentGateway.ExpensiveGateway;
using PaymentRepository.PaymentRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentRepository
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddPaymentRepository(this IServiceCollection services)
        {
            services.AddScoped<ICheapPaymentGateway, CheapPaymentGateway>();
            services.AddScoped<IExpensivePaymentGateway, ExpensivePaymentGateway>();
            services.AddScoped<IPaymentServiceRepository, PaymentServiceRepository>();

            return services;
        }
    }
}
