using PaymentAppService.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentAppService.Validations
{
    public static class ValidationExtension
    {
        public static bool ValidateRequest(this PaymentDto payment)
        {
            if (string.IsNullOrEmpty(payment.CardHolder))
            {
                return false;
            }

            if (payment.ExpirationDate < DateTime.Now || payment.ExpirationDate == null)
            {
                return false;
            }

            if (payment.SecurityCode.Length > 3)
            {
                return false;
            }

            if (payment.Amount < 0)
            {
                return false;
            }

            if(payment.CreditCardNumber.Length != 16)
            {
                return false;
            }

            return true;
        }
    }
}
