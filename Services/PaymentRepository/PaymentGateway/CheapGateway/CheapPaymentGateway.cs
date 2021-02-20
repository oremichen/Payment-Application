using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentRepository.PaymentGateway.CheapGateway
{
    public class CheapPaymentGateway : ICheapPaymentGateway
    {
        public async Task ExecuteCheapPaymentService(decimal amount)
        {
            await Task.CompletedTask;
        }
    }
}
