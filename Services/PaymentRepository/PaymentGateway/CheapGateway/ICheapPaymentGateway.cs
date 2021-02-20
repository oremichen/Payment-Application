using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentRepository.PaymentGateway.CheapGateway
{
    public interface ICheapPaymentGateway
    {
        Task ExecuteCheapPaymentService(decimal amount);
    }
}
