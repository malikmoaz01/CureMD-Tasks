using System;
using ECommerce2.Models;

namespace ECommerce2.Strategies
{
    public class CryptoPayment : IPaymentStrategy
    {
        public string WalletAddress { get; set; }
        public string CryptoCurrency { get; set; }

        public CryptoPayment(string walletAddress, string cryptoCurrency)
        {
            WalletAddress = walletAddress;
            CryptoCurrency = cryptoCurrency;
        }

        public bool ValidatePaymentDetails()
        {
            if (WalletAddress == null || WalletAddress.Length < 26)
                return false;
            if (CryptoCurrency == null)
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
                    Message = "Invalid crypto wallet details"
                };
            }

            return new PaymentResult
            {
                IsSuccessful = true,
                Message = $"Cryptocurrency payment of ${amount} processed successfully",
                TransactionId = "CR" + DateTime.Now.Ticks.ToString().Substring(10)
            };
        }
    }
}
