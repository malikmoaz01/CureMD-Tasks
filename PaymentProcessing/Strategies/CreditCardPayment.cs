using System;
using PaymentProcessing.Abstractions;
using PaymentProcessing.Models;

namespace PaymentProcessing.Strategies
{
    public class CreditCardPayment : IPaymentStrategy
    {
        public void ProcessPayment(Order order)
        {
            Console.WriteLine($"Processing Credit Card payment for Order {order.OrderId}");
        }
    }
}