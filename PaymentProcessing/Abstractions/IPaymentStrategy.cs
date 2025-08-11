using System;

namespace PaymentProcessing.Abstractions
{
    public interface IPaymentStrategy
    {
        void ProcessPayment(Order order);
    }
}