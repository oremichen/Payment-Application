using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static PaymentCore.ENUM;

namespace PaymentCore
{
    public class Payments
    {
        [Key]
        public long PaymentId { get; set; }

        [Required]
        public string CreditCardNumber { get; set; }

        [Required]
        public string CardHolder { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [StringLength(3)]
        public string SecurityCode { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string Status { get; set; }
    }

   
}
