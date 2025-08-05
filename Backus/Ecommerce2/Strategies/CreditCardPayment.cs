using System;
using ECommerce2.Models;

namespace ECommerce2.Strategies
{
    public class CreditCardPayment : IPaymentStrategy
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        public string HolderName { get; set; }

        public CreditCardPayment(string cardNumber, string expiryDate, string cvv, string holderName)
        {
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            CVV = cvv;
            HolderName = holderName;
        }

        public bool ValidatePaymentDetails()
        {
            if (CardNumber == null || CardNumber.Length != 16)
                return false;
            if (CVV == null || CVV.Length != 3)
                return false;
            if (ExpiryDate == null || ExpiryDate.Length != 5)
                return false;
            return true;
        }

        public PaymentResult ProcessPayment(decimal amount)
        {
            if (!ValidatePaymentDetails())
            {
                return new PaymentResult
                {
                    IsSuccessful = false,
                    Message = "Invalid credit card details"
                };
            }

            return new PaymentResult
            {
                IsSuccessful = true,
                Message = $"Credit card payment of ${amount} processed successfully",
                TransactionId = "CC" + DateTime.Now.Ticks.ToString().Substring(10)
            };
        }
    }
}
