using PaymentCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentRepository.PaymentRepo
{
    public interface IPaymentServiceRepository
    {
        Task<long> InsertPayment(Payments payments);

        Task<Payments> GetPaymentsByPaymentId(long paymentId);

        Task UpdatePayment(Payments payments);
    }
}
