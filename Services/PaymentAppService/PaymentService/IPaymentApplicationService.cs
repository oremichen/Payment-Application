using PaymentAppService.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAppService.PaymentService
{
    public interface IPaymentApplicationService
    {
        Task<bool> ExecuteProcessPayment(PaymentDto payment); 
    }
}
