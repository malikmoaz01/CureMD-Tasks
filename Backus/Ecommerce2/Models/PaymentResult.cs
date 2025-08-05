using System;
using System.Collections.Generic;

namespace ECommerce2.Models
{
    public class PaymentResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
    }
}