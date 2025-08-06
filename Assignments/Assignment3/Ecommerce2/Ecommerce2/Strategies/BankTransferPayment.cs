using System;
using ECommerce2.Models;

namespace ECommerce2.Strategies
{
    public class BankTransferPayment : IPaymentStrategy
    {
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }

        public BankTransferPayment(string routingNumber, string accountNumber, string bankName)
        {
            RoutingNumber = routingNumber;
            AccountNumber = accountNumber;
            BankName = bankName;
        }

        public bool ValidatePaymentDetails()
        {
            if (RoutingNumber == null || RoutingNumber.Length != 9)
                return false;
            if (AccountNumber == null || AccountNumber.Length < 8)
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
                    Message = "Invalid bank transfer details"
                };
            }

            return new PaymentResult
            {
                IsSuccessful = true,
                Message = $"Bank transfer of Rs {amount} processed successfully",
                TransactionId = "BT" + DateTime.Now.Ticks.ToString().Substring(10)
            };
        }
    }
}