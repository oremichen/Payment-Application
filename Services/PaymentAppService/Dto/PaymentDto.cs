using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentAppService.Dto
{
    public class PaymentDto
    {
        public string CreditCardNumber { get; set; }

        public string CardHolder { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string SecurityCode { get; set; }

        public decimal Amount { get; set; }

    }
}
