using System;
using PaymentProcessing.Abstractions;
using PaymentProcessing.Strategies;

namespace PaymentProcessing.Factories
{
    public class PaymentStrategyFactory
    {
        public IPaymentStrategy GetPaymentStrategy(string paymentType)
        {
            return paymentType switch
            {
                "CreditCard" => new CreditCardPayment(),
                "PayPal" => new PayPalPayment(),
                _ => throw new NotSupportedException($"Payment type {paymentType} not supported")
            };
        }
    }
}