using Microsoft.EntityFrameworkCore;
using PaymentCore;
using PaymentRepository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentRepository.PaymentRepo
{
    public class PaymentServiceRepository : IPaymentServiceRepository
    {
        private readonly PaymentDb _paymentDb;

        public PaymentServiceRepository(PaymentDb paymentDb)
        {
            _paymentDb = paymentDb;
        }

        public async Task<Payments> GetPaymentsByPaymentId(long paymentId)
        {
            try
            {
                var result = await _paymentDb.Payments.Where(p => p.PaymentId == paymentId).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<long> InsertPayment(Payments payments)
        {
            try
            {
                var result =  await _paymentDb.Payments.AddAsync(payments);
                await _paymentDb.SaveChangesAsync();

                return result.Entity.PaymentId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdatePayment(Payments payments)
        {
            try
            {
                var result =  _paymentDb.Payments.Update(payments);
                await _paymentDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
