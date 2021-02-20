using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentRepository.PaymentGateway.ExpensiveGateway
{
    public interface IExpensivePaymentGateway
    {
        Task<bool> PremiumPaymentService(decimal amount);

        Task<bool> ExpensiveaymentService(decimal amount);
    }
}
