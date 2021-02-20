using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentRepository.PaymentGateway.ExpensiveGateway
{
    public class ExpensivePaymentGateway : IExpensivePaymentGateway
    {
        public async Task<bool> ExpensiveaymentService(decimal amount)
        {
            if (amount > 500)
            {
                return false;
            }
            return true;

        }

        public async Task<bool> PremiumPaymentService(decimal amount)
        {
            return false;
        }
    }
}
