using System;
using System.Text.RegularExpressions;

namespace ECommerceOrderSystem.Strategies
{
    public interface IPaymentStrategy
    {
        bool ProcessPayment(decimal amount);
        bool ValidatePaymentInfo();
        string GetPaymentMethod();
    }

    public class PaymentContext
    {
        private IPaymentStrategy _paymentStrategy;

        public void SetPaymentStrategy(IPaymentStrategy strategy)
        {
            _paymentStrategy = strategy;
        }

        public bool ProcessPayment(decimal amount)
        {
            if (_paymentStrategy == null)
                throw new InvalidOperationException("Payment strategy not set");

            if (!_paymentStrategy.ValidatePaymentInfo())
                return false;

            return _paymentStrategy.ProcessPayment(amount);
        }
    }

    public class CreditCardPayment : IPaymentStrategy
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }

        public CreditCardPayment(string cardNumber, string expiryDate, string cvv)
        {
            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            CVV = cvv;
        }

        public bool ValidatePaymentInfo()
        {
            if (string.IsNullOrEmpty(CardNumber) || CardNumber.Length != 16)
                return false;

            if (string.IsNullOrEmpty(CVV) || CVV.Length != 3)
                return false;

            if (string.IsNullOrEmpty(ExpiryDate) || !Regex.IsMatch(ExpiryDate, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                return false;

            return true;
        }

        public bool ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Processing Credit Card payment of ${amount}");
            Console.WriteLine($"Card ending in: ****{CardNumber.Substring(CardNumber.Length - 4)}");
            return true;
        }

        public string GetPaymentMethod()
        {
            return "Credit Card";
        }
    }

    public class PayPalPayment : IPaymentStrategy
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public PayPalPayment(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public bool ValidatePaymentInfo()
        {
            if (string.IsNullOrEmpty(Email) || !Email.Contains("@"))
                return false;

            if (string.IsNullOrEmpty(Password) || Password.Length < 6)
                return false;

            return true;
        }

        public bool ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Processing PayPal payment of ${amount}");
            Console.WriteLine($"PayPal account: {Email}");
            return true;
        }

        public string GetPaymentMethod()
        {
            return "PayPal";
        }
    }

    public class BankTransferPayment : IPaymentStrategy
    {
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }

        public BankTransferPayment(string routingNumber, string accountNumber)
        {
            RoutingNumber = routingNumber;
            AccountNumber = accountNumber;
        }

        public bool ValidatePaymentInfo()
        {
            if (string.IsNullOrEmpty(RoutingNumber) || RoutingNumber.Length != 9)
                return false;

            if (string.IsNullOrEmpty(AccountNumber) || AccountNumber.Length < 8)
                return false;

            return true;
        }

        public bool ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Processing Bank Transfer payment of ${amount}");
            Console.WriteLine($"Account ending in: ****{AccountNumber.Substring(AccountNumber.Length - 4)}");
            return true;
        }

        public string GetPaymentMethod()
        {
            return "Bank Transfer";
        }
    }

    public class CryptoPayment : IPaymentStrategy
    {
        public string WalletAddress { get; set; }

        public CryptoPayment(string walletAddress)
        {
            WalletAddress = walletAddress;
        }

        public bool ValidatePaymentInfo()
        {
            if (string.IsNullOrEmpty(WalletAddress) || WalletAddress.Length < 26)
                return false;

            return true;
        }

        public bool ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Processing Cryptocurrency payment of ${amount}");
            Console.WriteLine($"Wallet: {WalletAddress.Substring(0, 8)}...{WalletAddress.Substring(WalletAddress.Length - 8)}");
            return true;
        }

        public string GetPaymentMethod()
        {
            return "Cryptocurrency";
        }
    }
}