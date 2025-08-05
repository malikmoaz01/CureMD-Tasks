using System;
using ECommerce2.Models;

namespace ECommerce2.Strategies
{
    public class PayPalPayment : IPaymentStrategy
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public PayPalPayment(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public bool ValidatePaymentDetails()
        {
            if (Email == null || !Email.Contains("@"))
                return false;
            if (Password == null || Password.Length < 6)
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
                    Message = "Invalid PayPal credentials"
                };
            }

            return new PaymentResult
            {
                IsSuccessful = true,
                Message = $"PayPal payment of ${amount} processed successfully",
                TransactionId = "PP" + DateTime.Now.Ticks.ToString().Substring(10)
            };
        }
    }
}